using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml;

namespace SimpleReport
{
    public static class Info
    {
        private static readonly Random random = new();
        public static int CashierId { get; set; }
        public static int UserType { get; set; }

        public static bool IsAllDigits(string s)
        {
            foreach (char c in s)
            {
                if (!char.IsDigit(c))
                    return false;
            }

            return true;
        }

        public static string CleanString(string Input)
        {
            return Input.Replace("/", string.Empty).Replace(" ", string.Empty);
        }

        public static void ToCapital(TextBox txt)
        {
            txt.Text = txt.Text.ToUpper();
            txt.Select(txt.Text.Length, 0);
        }

        public static void CmbToCapital(ComboBox txt)
        {
            txt.Text = txt.Text.ToUpper();
            txt.Select(txt.Text.Length, 0);
        }

        public static void Mes(string mes)
        {
            MessageBox.Show(mes);
        }

        public static int ToInt(TextBox tb)
        {
            if (IsEmpty(tb))
            {
                return Convert.ToInt32(tb.Text.Trim());
            }
            else
            {
                return 0;
            }
        }

        public static bool YesNoConfirmation(string message, string title)
        {
            DialogResult dialogResult = MessageBox.Show(message, title, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                return true;
            }
            else
                return false;
        }

        public static void Leave(TextBox Txt, string PlaceHolder)
        {
            if (string.IsNullOrWhiteSpace(Txt.Text.Trim()))
            {
                Txt.Text = PlaceHolder;
                Txt.ForeColor = System.Drawing.Color.Black;
            }
        }

        public static void PlaceHolder(TextBox tb)
        {
            if (tb.Focused)
            {
                tb.Text = string.Empty;
                tb.ForeColor = System.Drawing.Color.Black;
            }
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static void Required()
        {
            MessageBox.Show("Please make sure you have entered all the required fields");
        }

        public static DateTime Today()
        {
            string Date = DateTime.Today.ToShortDateString();
            DateTime Today = Convert.ToDateTime(Date);
            return Today;
        }

        public static bool IsNumber(char ch, string text)
        {
            bool res = true;
            char decimalChar = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            if (ch == decimalChar && text.IndexOf(decimalChar) != -1)
            {
                res = false;
                return res;
            }

            if (!char.IsDigit(ch) && ch != decimalChar && ch != (char)Keys.Back)
                res = false;

            return res;
        }

        public static void IsDecimal(KeyPressEventArgs e, TextBox txt)
        {
            if (!IsNumber(e.KeyChar, txt.Text))
                e.Handled = true;
        }

        public static DataTable DGVToDataTable(DataGridView dataGridView)
        {
            var dt = new DataTable();
            foreach (DataGridViewColumn dataGridViewColumn in dataGridView.Columns)
            {
                if (dataGridViewColumn.Visible)
                {
                    dt.Columns.Add();
                }
            }

            var cell = new object[dataGridView.Columns.Count];
            foreach (DataGridViewRow dataGridViewRow in dataGridView.Rows)
            {
                for (int i = 0; i < dataGridViewRow.Cells.Count; i++)
                {
                    cell[i] = dataGridViewRow.Cells[i].Value;
                }

                dt.Rows.Add(cell);
            }

            return dt;
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                var type = prop.PropertyType.IsGenericType &&
                           prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                    ? Nullable.GetUnderlyingType(prop.PropertyType)
                    : prop.PropertyType;
                dataTable.Columns.Add(prop.Name, type);
            }

            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static void IsInt(KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(Keys.Back))
                e.Handled = true;
        }

        public static string ToString(TextBox tb)
        {
            if (IsEmpty(tb))
            {
                return tb.Text.Trim();
            }
            else
            {
                return string.Empty;
            }
        }

        public static bool IsEmpty(TextBox t)
        {
            if (string.IsNullOrWhiteSpace(t.Text.Trim()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void ExportReceiptsAsPDF(DataTable dt)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "PDF (*.pdf)|*.pdf",
                    FileName = RandomString(5) + DateTime.Now.ToShortDateString().Replace("/", "")
                };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string fileName = sfd.FileName;
                    PdfWriter writer = new PdfWriter(sfd.FileName);
                    PdfDocument pdf = new PdfDocument(writer);
                    Document document = new Document(pdf);
                    Paragraph header = new Paragraph("Receipts").SetTextAlignment(TextAlignment.CENTER).SetFontSize(20);
                    Paragraph subheader = new Paragraph("Detailed Report").SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(15);
                    Paragraph dl = new Paragraph("                ").SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(15);
                    LineSeparator ls = new LineSeparator(new SolidLine());
                    document.Add(header);
                    document.Add(subheader);
                    document.Add(ls);
                    document.Add(dl);
                    Table table = new Table(6, false);
                    table.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);
                    table.SetVerticalAlignment(VerticalAlignment.TOP);
                    table.AddHeaderCell("Receipt No");
                    table.AddHeaderCell("Date");
                    table.AddHeaderCell("Time");
                    table.AddHeaderCell("Sub Total");
                    table.AddHeaderCell("Total Discounts");
                    table.AddHeaderCell("Net Total");

                    foreach (DataRow d in dt.Rows)
                    {
                        table.AddCell(new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT)
                            .Add(new Paragraph(d[0].ToString())));
                        table.AddCell(new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT)
                            .SetBackgroundColor(ColorConstants.LIGHT_GRAY).Add(new Paragraph(d[1].ToString())));
                        table.AddCell(new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT)
                            .Add(new Paragraph(d[2].ToString())));
                        table.AddCell(new Cell(1, 1).SetTextAlignment(TextAlignment.RIGHT)
                            .SetBackgroundColor(ColorConstants.LIGHT_GRAY).Add(new Paragraph(d[4].ToString())));
                        table.AddCell(new Cell(1, 1).SetTextAlignment(TextAlignment.RIGHT)
                            .Add(new Paragraph(d[5].ToString())));
                        table.AddCell(new Cell(1, 1).SetTextAlignment(TextAlignment.RIGHT)
                            .SetBackgroundColor(ColorConstants.LIGHT_GRAY).Add(new Paragraph(d[6].ToString())));
                    }

                    float subTotal = GetDTSum(dt, 4);
                    float discoutTotal = GetDTSum(dt, 5);
                    float netTotal = GetDTSum(dt, 6);

                    table.AddFooterCell(new Cell(1, 1).SetTextAlignment(TextAlignment.RIGHT)
                        .SetBackgroundColor(ColorConstants.GRAY).Add(new Paragraph(string.Empty)));
                    table.AddFooterCell(new Cell(1, 1).SetTextAlignment(TextAlignment.RIGHT)
                        .SetBackgroundColor(ColorConstants.GRAY).Add(new Paragraph(string.Empty)));
                    table.AddFooterCell(new Cell(1, 1).SetTextAlignment(TextAlignment.RIGHT)
                        .SetBackgroundColor(ColorConstants.GRAY).Add(new Paragraph(string.Empty)));
                    table.AddFooterCell(new Cell(1, 1).SetTextAlignment(TextAlignment.RIGHT)
                        .SetBackgroundColor(ColorConstants.GRAY).Add(new Paragraph(subTotal.ToString())));
                    table.AddFooterCell(new Cell(1, 1).SetTextAlignment(TextAlignment.RIGHT)
                        .SetBackgroundColor(ColorConstants.GRAY).Add(new Paragraph(discoutTotal.ToString())));
                    table.AddFooterCell(new Cell(1, 1).SetTextAlignment(TextAlignment.RIGHT)
                        .SetBackgroundColor(ColorConstants.GRAY).Add(new Paragraph(netTotal.ToString())));

                    document.Add(table);
                    document.Close();
                    StartProcess(fileName);
                }
            }
            catch (Exception ex)
            {
                Mes(ex.Message);
            }
        }

        public static void SendToPrinter(string file)
        {
            ProcessStartInfo info = new ProcessStartInfo
            {
                Verb = "print",
                FileName = file,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process p = new Process
            {
                StartInfo = info
            };
            p.Start();
            p.WaitForInputIdle();
            System.Threading.Thread.Sleep(3000);
            if (!p.CloseMainWindow())
                p.Kill();
        }

        public static void StartProcess(string Path)
        {
            try
            {
                Process.Start(Path);
            }
            catch (Exception ex)
            {
                Mes(ex.Message);
            }
        }

        public static void ExportAsExcel(DataGridView Dgv)
        {
            if (Dgv.Rows.Count > 0)
            {
                Microsoft.Office.Interop.Excel.Application xcelApp = new Microsoft.Office.Interop.Excel.Application();
                xcelApp.Application.Workbooks.Add(Type.Missing);
                for (int i = 1; i < Dgv.Columns.Count + 1; i++)
                {
                    xcelApp.Cells[1, i] = Dgv.Columns[i - 1].HeaderText;
                }

                for (int i = 0; i < Dgv.Rows.Count; i++)
                {
                    for (int j = 0; j < Dgv.Columns.Count; j++)
                    {
                        xcelApp.Cells[i + 2, j + 1] = Dgv.Rows[i].Cells[j].Value.ToString();
                    }
                }

                xcelApp.Columns.AutoFit();
                xcelApp.Visible = true;
            }
        }

        public static void ExportAsPdf(DataTable dgv)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PDF (*.pdf)|*.pdf",
                FileName = "test"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Spire.DataExport.PDF.PDFExport PDFExport = new Spire.DataExport.PDF.PDFExport
                {
                    DataSource = Spire.DataExport.Common.ExportSource.DataTable,
                    DataTable = dgv,
                    ActionAfterExport = Spire.DataExport.Common.ActionType.OpenView
                };

                PDFExport.PDFOptions.PageOptions.Orientation = Spire.DataExport.Common.PageOrientation.Landscape;
                PDFExport.SaveToFile(sfd.FileName);
            }
        }

        public static void ExpPDF(DataTable dt)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PDF (*.pdf)|*.pdf",
                FileName = "test"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Spire.DataExport.PDF.PDFExport PDFExport = new Spire.DataExport.PDF.PDFExport
                {
                    DataSource = Spire.DataExport.Common.ExportSource.DataTable,
                    DataTable = dt,
                    ActionAfterExport = Spire.DataExport.Common.ActionType.OpenView
                };
                PDFExport.PDFOptions.PageOptions.Orientation = Spire.DataExport.Common.PageOrientation.Landscape;
                PDFExport.SaveToFile(sfd.FileName);
            }
        }

        public static void ExportPurchaseOrders(DataTable dt, string Date)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "PDF (*.pdf)|*.pdf",
                    FileName = RandomString(5) + DateTime.Now.ToShortDateString().Replace("/", "")
                };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string fileName = sfd.FileName;
                    PdfWriter writer = new PdfWriter(sfd.FileName);
                    PdfDocument pdf = new PdfDocument(writer);
                    Document document = new Document(pdf, iText.Kernel.Geom.PageSize.A4);
                    Paragraph header = new Paragraph("Ordered Items").SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(15);
                    Paragraph subheader = new Paragraph("Oder due date : " + Date)
                        .SetTextAlignment(TextAlignment.CENTER).SetFontSize(12);
                    Paragraph dl = new Paragraph(".       .").SetTextAlignment(TextAlignment.CENTER).SetFontSize(10)
                        .SetFontColor(ColorConstants.WHITE);
                    LineSeparator ls = new LineSeparator(new SolidLine());
                    document.Add(header);
                    document.Add(subheader);
                    document.Add(ls);
                    document.Add(dl);
                    int FontSize = 10;
                    Table table = new Table(3, false);
                    table.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);
                    table.SetVerticalAlignment(VerticalAlignment.TOP);
                    table.AddHeaderCell("Item Code").SetFontSize(FontSize);
                    table.AddHeaderCell("Item Name").SetFontSize(FontSize);
                    table.AddHeaderCell("Quantity").SetFontSize(FontSize);

                    foreach (DataRow d in dt.Rows)
                    {
                        table.AddCell(new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).SetFontSize(FontSize)
                            .Add(new Paragraph(d[0].ToString())));
                        table.AddCell(new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).SetFontSize(FontSize)
                            .Add(new Paragraph(d[1].ToString())));
                        table.AddCell(new Cell(1, 1).SetTextAlignment(TextAlignment.RIGHT).SetFontSize(FontSize)
                            .Add(new Paragraph(d[2].ToString())));
                    }

                    int TotalOrderedQty = GetDTIntegerSum(dt, 2);
                    table.AddFooterCell(new Cell(1, 1).SetTextAlignment(TextAlignment.RIGHT).SetFontSize(FontSize)
                        .Add(new Paragraph(string.Empty)));
                    table.AddFooterCell(new Cell(1, 1).SetTextAlignment(TextAlignment.RIGHT).SetFontSize(FontSize)
                        .Add(new Paragraph("Total Ordered Quantity")));
                    table.AddFooterCell(new Cell(1, 1).SetTextAlignment(TextAlignment.RIGHT).SetFontSize(FontSize)
                        .Add(new Paragraph(TotalOrderedQty.ToString())));
                    document.Add(table);
                    document.Close();
                    StartProcess(fileName);
                }
            }
            catch (Exception ex)
            {
                Mes(ex.Message);
            }
        }

        public static float GetDGVSum(DataGridView dt, int cell)
        {
            float sum = (from DataGridViewRow row in dt.Rows
                where row.Cells[0].FormattedValue.ToString() != string.Empty
                select Convert.ToSingle(row.Cells[cell].FormattedValue)).Sum();
            return sum;
        }

        public static float GetDTSum(DataTable dt, int cell)
        {
            float sum = dt.AsEnumerable().Sum(c => c.Field<float>(cell));
            return sum;
        }

        public static int GetDTIntegerSum(DataTable dt, int cell)
        {
            int sum = dt.AsEnumerable().Sum(c => c.Field<int>(cell));
            return sum;
        }

        public static void Add(Exception ex)
        {
            try
            {
                using (BillingContext db = new BillingContext())
                {
                    var data = db.Settings.Take(1).FirstOrDefault();
                    if (data == null) return;
                    string path = data.ExceptionPath;
                    string fileName = path + "exception.json";
                    string rawJson;
                    if (!File.Exists(fileName))
                    {
                        File.Create(fileName);
                    }

                    rawJson = File.ReadAllText(fileName);
                    var ec = JsonConvert.DeserializeObject<ExpCollection>(rawJson);

                    if (ec != null)
                    {
                        int Count = ec.Exceptions.Count;
                        Exp Exps = new Exp
                        {
                            Id = ++Count,
                            Time = DateTime.Now.ToShortTimeString(),
                            Date = DateTime.Today.ToShortDateString(),
                            Message = ex.Message.ToString(),
                            StackTrace = ex.StackTrace.ToString()
                        };
                        ec.Exceptions.Add(Exps);
                        string serializedJson = JsonConvert.SerializeObject(ec, Formatting.Indented);
                        File.WriteAllText(path, serializedJson);
                    }
                    else
                    {
                        ExpCollection exp = new ExpCollection
                        {
                            Exceptions = new List<Exp>()
                        };
                        Exp Exps = new Exp
                        {
                            Id = 1,
                            Time = DateTime.Now.ToShortTimeString(),
                            Date = DateTime.Today.ToShortDateString(),
                            Message = ex.Message.ToString(),
                            StackTrace = ex.StackTrace.ToString()
                        };
                        exp.Exceptions.Add(Exps);
                        string serializedJson = JsonConvert.SerializeObject(exp, Formatting.Indented);
                        File.WriteAllText(path, serializedJson);
                    }
                }
            }
            catch (Exception e)
            {
                Add(e);
            }
            finally
            {
                Mes(ex.Message);
            }
        }

        public static DbConnection ConString()
        {
            try
            {
                string fileName = "conString.json";
                string rawJson = File.ReadAllText(fileName);
                var cs = JsonConvert.DeserializeObject<ConnectionString>(rawJson);
                return GetSqlConnection(cs.Source, cs.Database, cs.UserId, cs.Password, cs.IntegratedSecurity);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static DbConnection GetSqlConnection(string dataSource, string dbName, string uId, string pW,
            bool trusted)
        {
            var sqlConnStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = dataSource,
                MultipleActiveResultSets = true
            };

            if (trusted)
            {
                sqlConnStringBuilder.IntegratedSecurity = true;
            }
            else
            {
                sqlConnStringBuilder.UserID = uId;
                sqlConnStringBuilder.Password = pW;
            }

            var sqlConnFact = new SqlConnectionFactory(sqlConnStringBuilder.ConnectionString);
            var sqlConn = sqlConnFact.CreateConnection(dbName);
            return sqlConn;
        }
    }
}