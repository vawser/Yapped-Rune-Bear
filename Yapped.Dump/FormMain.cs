using OfficeOpenXml;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using CellType = SoulsFormats.PARAM.CellType;

namespace Yapped.Dump
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void btnDump_Click(object sender, EventArgs e)
        {
            BND4 bnd;
            try
            {
                bnd = SFUtil.DecryptDS3Regulation(txtRegulation.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load regulation:\r\n\r\n{txtRegulation.Text}\r\n\r\n{ex}");
                return;
            }

            var translations = new Dictionary<string, string>();
            var xml = new XmlDocument();
            xml.Load("translations.xml");

            foreach (XmlNode text in xml.SelectNodes("translations/text"))
            {
                string jp = text.SelectSingleNode("jp").InnerText;
                string en = text.SelectSingleNode("en").InnerText;
                translations[WebUtility.HtmlDecode(jp)] = WebUtility.HtmlDecode(en);
            }

            var package = new ExcelPackage();

            foreach (BinderFile file in bnd.Files)
            {
                if (Path.GetExtension(file.Name) == ".param")
                {
                    PARAM param = PARAM.Read(file.Bytes);
                    string layoutPath = $"Layouts\\{param.ID}.xml";

                    txtStatus.AppendText(file.Name + "\r\n");

                    var worksheet = package.Workbook.Worksheets.Add(Path.GetFileNameWithoutExtension(file.Name));

                    PARAM.Layout layout;
                    if (File.Exists(layoutPath))
                    {
                        layout = PARAM.Layout.ReadXMLFile(layoutPath);
                        if (layout.Size != param.DetectedSize)
                        {
                            layout = new PARAM.Layout();
                            for (int i = 0; i < param.DetectedSize / 4; i++)
                                layout.Add(new PARAM.Layout.Entry(CellType.u32, $"unk0x{i * 4:X4}", (uint)0));
                            for (int i = 0; i < param.DetectedSize % 4; i++)
                                layout.Add(new PARAM.Layout.Entry(CellType.u8, "unkb" + i, (byte)0));
                        }
                    }
                    else
                    {
                        layout = new PARAM.Layout();
                    }

                    param.SetLayout(layout);
                    List<PARAM.Row> rows = param.Rows;

                    worksheet.Cells[1, 1].Value = "ID";
                    worksheet.Cells[1, 2].Value = "Name";
                    worksheet.Cells[1, 3].Value = "Translated";
                    int columnCount = 3;
                    foreach (PARAM.Layout.Entry lv in layout)
                        if (lv.Type != CellType.dummy8)
                            worksheet.Cells[1, ++columnCount].Value = lv.Name;

                    for (int i = 0; i < rows.Count; i++)
                    {
                        PARAM.Row row = rows[i];
                        worksheet.Cells[i + 2, 1].Value = row.ID;
                        if (row.Name != null)
                        {
                            if (translations.ContainsKey(row.Name))
                            {
                                worksheet.Cells[i + 2, 2].Value = row.Name;
                                worksheet.Cells[i + 2, 3].Value = translations[row.Name];
                            }
                            else if (row.Name.Contains(" -- "))
                            {
                                worksheet.Cells[i + 2, 2].Value = row.Name.Substring(row.Name.IndexOf(" -- ") + 4);
                                worksheet.Cells[i + 2, 3].Value = row.Name.Substring(0, row.Name.IndexOf(" -- "));
                            }
                        }
                        else
                        {
                            worksheet.Cells[i + 2, 2].Value = row.Name;
                        }
                        columnCount = 3;

                        foreach (PARAM.Cell cell in row.Cells)
                        {
                            CellType type = cell.Type;
                            if (type != CellType.dummy8)
                            {
                                var range = worksheet.Cells[i + 2, ++columnCount];
                                if (type == CellType.f32)
                                {
                                    range.Value = (double)(float)cell.Value;
                                }
                                else if (type == CellType.b8 || type == CellType.b16 || type == CellType.b32)
                                {
                                    bool b = (bool)cell.Value;
                                    range.Value = b.ToString();
                                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    range.Style.Fill.BackgroundColor.SetColor(b ? Color.LightGreen : Color.LightPink);
                                }
                                else if (type == CellType.x8)
                                {
                                    range.Value = $"0x{cell.Value:X2}";
                                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                                }
                                else if (type == CellType.x16)
                                {
                                    range.Value = $"0x{cell.Value:X4}";
                                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                                }
                                else if (type == CellType.x32)
                                {
                                    range.Value = $"0x{cell.Value:X8}";
                                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                                }
                                else
                                {
                                    range.Value = cell.Value;
                                }
                            }
                        }
                    }

                    worksheet.Row(1).Style.Font.Bold = true;
                    worksheet.Column(1).Style.Font.Bold = true;
                    worksheet.View.FreezePanes(2, 4);
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                }
            }

            FileInfo f = new FileInfo(Path.Combine(txtOutput.Text, "dump.xlsx"));
            package.SaveAs(f);
        }
    }
}
