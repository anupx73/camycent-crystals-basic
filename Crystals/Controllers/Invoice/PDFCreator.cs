using System;
using System.Collections.Generic;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Drawing.Imaging;
using System.Data.SqlClient;

namespace Crystals.Controllers
{
    public class PDFCreator
    {
        //private static string watermarkImage = "WM.jpg";// Watermark image shld be JPG, set in here

        SqlDataReader reader = Program.db.get("Company");
        
        private static string logoImagePng = "";

        public static int noOfInfoColumns = 5;
        // Set up the fonts to be used on the pages
        private iTextSharp.text.Font _largeFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 18, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
        private iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
        private iTextSharp.text.Font _smallFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
        
        public static string invNo = "";
        public static List<string> cust_Info, sales_Info;

        Dictionary<string, List> invoiceMapper = new Dictionary<string, List>();

        public bool Build(string fileName, string invoiceNo, List<string> custInfo, List<string> salesInfo, bool status)
        {
            iTextSharp.text.Document doc = null;
            cust_Info = new List<string>();
            sales_Info = new List<string>();
            
            cust_Info.AddRange(custInfo);
            sales_Info.AddRange(salesInfo);
            reader.Read();
            logoImagePng = reader["Logo"].ToString();
            string address = reader["Address"].ToString();
            string phone = reader["Phone"].ToString();

            int compAddrOffsetXStart = 60,
                compAddrOffsetXEnd = 210,
                compAddrOffsetYStart = 660,
                compAddrOffsetYEnd = 670,
                invOffsetXStart = 420,
                invOffsetYStart = 730,
                invOffsetXEnd = 528,
                invOffsetYEnd = 740,
                footerOffsetXStart = 120,
                footerOffsetYStart = 120,
                footerOffsetXEnd = 500,
                footerOffsetYEnd = 140;

            try
            {
                // Initialize the PDF document
                doc = new Document(PageSize.A4);
                //string pdfPath = System.IO.Directory.GetCurrentDirectory() + "\\InvoiceRepo" + fileName;
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc,
                    new System.IO.FileStream(System.IO.Directory.GetCurrentDirectory() + "\\InvoiceRepo" + fileName,
                        System.IO.FileMode.Create));

                doc.Open();
                
                PdfContentByte cb = writer.DirectContent;
                ColumnText ct = new ColumnText(cb);

                // all elements except grid table drawn first

             //   ct.SetSimpleColumn(new Phrase(new Chunk(address, FontFactory.GetFont(FontFactory.HELVETICA,
                                //   8, iTextSharp.text.Font.NORMAL))), compAddrOffsetXStart, compAddrOffsetYStart-0, compAddrOffsetXEnd, 
                                //   compAddrOffsetYEnd-10, 0, Element.ALIGN_LEFT | Element.ALIGN_TOP);
              // ct.Go();
                ct.SetSimpleColumn(new Phrase(new Chunk(address, FontFactory.GetFont(FontFactory.HELVETICA,
                                   8, iTextSharp.text.Font.NORMAL))), compAddrOffsetXStart, compAddrOffsetYStart, compAddrOffsetXEnd+100,
                                   compAddrOffsetYEnd - 10, 0, Element.ALIGN_LEFT | Element.ALIGN_TOP);
                ct.Go();
                ct.SetSimpleColumn(new Phrase(new Chunk(phone, FontFactory.GetFont(FontFactory.HELVETICA,
                                   8, iTextSharp.text.Font.NORMAL))), compAddrOffsetXStart, compAddrOffsetYStart - 20, compAddrOffsetXEnd,
                                   compAddrOffsetYEnd - 20, 0, Element.ALIGN_LEFT | Element.ALIGN_TOP);
                ct.Go();
                ct.SetSimpleColumn(new Phrase(new Chunk("RELAX - REJUVINATE - REFRESH", FontFactory.GetFont(FontFactory.HELVETICA,
                                   8, iTextSharp.text.Font.NORMAL))), compAddrOffsetXStart, compAddrOffsetYStart - 30, compAddrOffsetXEnd,
                                   compAddrOffsetYEnd - 30, 0, Element.ALIGN_LEFT | Element.ALIGN_TOP);
                ct.Go();
                ct.SetSimpleColumn(new Phrase(new Chunk("DAY SPA", FontFactory.GetFont(FontFactory.HELVETICA,
                                   8, iTextSharp.text.Font.NORMAL))), compAddrOffsetXStart, compAddrOffsetYStart - 40, compAddrOffsetXEnd,
                                   compAddrOffsetYEnd - 40, 0, Element.ALIGN_LEFT | Element.ALIGN_TOP);
                ct.Go();

                ct.SetSimpleColumn(new Phrase(new Chunk("Invoice", FontFactory.GetFont(FontFactory.HELVETICA, 20, iTextSharp.text.Font.NORMAL))),
                                   invOffsetXStart, invOffsetYStart, invOffsetXEnd, invOffsetYEnd, 0, Element.ALIGN_JUSTIFIED_ALL | Element.ALIGN_TOP);
                ct.Alignment = Element.ALIGN_RIGHT;
                ct.Go();

                ct.SetSimpleColumn(new Phrase(new Chunk("Date: " + cust_Info[0], FontFactory.GetFont(FontFactory.HELVETICA,
                                   8, iTextSharp.text.Font.NORMAL))), invOffsetXStart, invOffsetYStart - 20, invOffsetXEnd, invOffsetYEnd - 20, 0,
                                   Element.ALIGN_JUSTIFIED_ALL | Element.ALIGN_TOP);
                ct.Alignment = Element.ALIGN_RIGHT;
                ct.Go();
                ct.SetSimpleColumn(new Phrase(new Chunk("INVOICE #", FontFactory.GetFont(FontFactory.HELVETICA,
                   8, iTextSharp.text.Font.NORMAL))), invOffsetXStart, invOffsetYStart - 30, invOffsetXEnd, invOffsetYEnd - 30, 0,
                   Element.ALIGN_JUSTIFIED_ALL | Element.ALIGN_TOP);
                ct.Alignment = Element.ALIGN_RIGHT;
                ct.Go();
                ct.SetSimpleColumn(new Phrase(new Chunk(invoiceNo, FontFactory.GetFont(FontFactory.HELVETICA,
                   8, iTextSharp.text.Font.NORMAL))), invOffsetXStart, invOffsetYStart - 40, invOffsetXEnd, invOffsetYEnd - 40, 0,
                   Element.ALIGN_JUSTIFIED_ALL | Element.ALIGN_TOP);
                ct.Alignment = Element.ALIGN_RIGHT;
                ct.Go();
                int found = -1; 
                i = 10;

                ct.SetSimpleColumn(new Phrase(new Chunk("TO "+cust_Info[3], FontFactory.GetFont(FontFactory.HELVETICA,
                   10, iTextSharp.text.Font.BOLD))), invOffsetXStart, invOffsetYStart - 75, invOffsetXEnd, invOffsetYEnd - 75, 0,
                   Element.ALIGN_LEFT | Element.ALIGN_TOP);
                ct.Alignment = Element.ALIGN_RIGHT;
                ct.Go();

                ct.SetSimpleColumn(new Phrase(new Chunk("Payment Mode " + cust_Info[13], FontFactory.GetFont(FontFactory.HELVETICA,
                   10, iTextSharp.text.Font.BOLD))), invOffsetXStart, invOffsetYStart - 85, invOffsetXEnd, invOffsetYEnd - 85, 0,
                   Element.ALIGN_LEFT | Element.ALIGN_TOP);
                ct.Alignment = Element.ALIGN_RIGHT;
                ct.Go();
                
                string trucated = cust_Info[2];
                string toPrint = "";
                for (; trucated.Length != 0 ; i += 10)
                {
                    found = trucated.IndexOfAny(new char[] { '\n', '\0' });
                    if (trucated.IndexOf('\n') >= 0)
                    {
                        toPrint = trucated.Substring(0, found + 1);
                    }
                    else
                    {
                        if (trucated.Length > 25)
                            trucated = trucated.Substring(0, 25) + "\n";
                        ct.SetSimpleColumn(new Phrase(new Chunk(trucated, FontFactory.GetFont(FontFactory.HELVETICA_BOLD,
                       7, iTextSharp.text.Font.BOLD))), invOffsetXStart - 20, (invOffsetYStart - 75) - i, invOffsetXEnd,
                       (invOffsetYEnd - 75) - i, 0, Element.ALIGN_RIGHT | Element.ALIGN_TOP);
                        ct.Alignment = Element.ALIGN_RIGHT;
                        ct.Go();
                        break;
                    }
                    if (toPrint.Length > 25)
                        toPrint = toPrint.Substring(0, 25) + "\n";
                    ct.SetSimpleColumn(new Phrase(new Chunk(toPrint, FontFactory.GetFont(FontFactory.HELVETICA_BOLD,
                       7, iTextSharp.text.Font.BOLD))), invOffsetXStart - 20, (invOffsetYStart - 75) - i, invOffsetXEnd,
                       (invOffsetYEnd - 75) - i, 0, Element.ALIGN_RIGHT | Element.ALIGN_TOP);
                    ct.Alignment = Element.ALIGN_RIGHT;    
                    ct.Go();
                        trucated = trucated.Substring(found + 1);
                }

                ct.SetSimpleColumn(new Phrase(new Chunk(cust_Info[5], FontFactory.GetFont(FontFactory.HELVETICA_BOLD,
                  8, iTextSharp.text.Font.NORMAL))), invOffsetXStart - 20, (invOffsetYStart - 75) - i - 10, invOffsetXEnd, (invOffsetYStart - 75) - i - 10, 0,
                  Element.ALIGN_RIGHT | Element.ALIGN_TOP);
                ct.Alignment = Element.ALIGN_RIGHT;
                ct.Go();
                ct.SetSimpleColumn(new Phrase(new Chunk(cust_Info[4], FontFactory.GetFont(FontFactory.HELVETICA_BOLD,
                  8, iTextSharp.text.Font.NORMAL))), invOffsetXStart - 20, (invOffsetYStart - 75) - i - 20, invOffsetXEnd, (invOffsetYStart - 75) - i - 20, 0,
                  Element.ALIGN_RIGHT | Element.ALIGN_TOP);
                ct.Alignment = Element.ALIGN_RIGHT;
                ct.Go();

                //
                ct.SetSimpleColumn(new Phrase(new Chunk(cust_Info[8], FontFactory.GetFont(FontFactory.HELVETICA_BOLD,
                  8, iTextSharp.text.Font.NORMAL))), footerOffsetXStart + 90, footerOffsetYStart + 3, footerOffsetXEnd, footerOffsetYEnd + 3, 0,
                  Element.ALIGN_RIGHT | Element.ALIGN_TOP);
                ct.Go();

                ct.SetSimpleColumn(new Phrase(new Chunk("Service Tax Number: "+reader["STaxNumber"], 
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD,
                  6, iTextSharp.text.Font.NORMAL))), footerOffsetXStart, footerOffsetYStart, footerOffsetXEnd, footerOffsetYEnd, 0,
                  Element.ALIGN_RIGHT | Element.ALIGN_TOP);
                ct.Go();

                ct.SetSimpleColumn(new Phrase(new Chunk("To accept this invoice, sign here and return: "
                                 + "__________________________________________________________________",
                                 FontFactory.GetFont(FontFactory.HELVETICA_BOLD,
                  6, iTextSharp.text.Font.NORMAL))), footerOffsetXStart, footerOffsetYStart - 20, footerOffsetXEnd, footerOffsetYEnd - 20, 0,
                  Element.ALIGN_RIGHT | Element.ALIGN_TOP);
                ct.Go();

                ct.SetSimpleColumn(new Phrase(new Chunk("Thank you for your business!",
                                 FontFactory.GetFont(FontFactory.HELVETICA_BOLD,
                  12, iTextSharp.text.Font.ITALIC))), footerOffsetXStart + 85, footerOffsetYStart - 50, footerOffsetXEnd, footerOffsetYEnd - 50, 0,
                  Element.ALIGN_LEFT | Element.ALIGN_TOP);
                ct.Go();

                // Now Add table to the document
                this.AddToTable(doc, cb, status);

                Process.Start(System.IO.Directory.GetCurrentDirectory() + "\\InvoiceRepo" + fileName);
            }
            catch (iTextSharp.text.DocumentException dex)
            {
                Log.AppError("Exception occured from Build, Message: " + dex.ToString());
                return false;
            }
            finally
            {
                // Clean up
               
                doc.Close();
                doc = null;
            }
            
            return true;
        }

        private void AddToTable(iTextSharp.text.Document doc, PdfContentByte cb, bool status)
        {

            // Add a logo
            if(logoImagePng != null && System.IO.File.Exists(logoImagePng))
            {
                iTextSharp.text.Image logoImage = iTextSharp.text.Image.GetInstance(logoImagePng);
                logoImage.Alignment = iTextSharp.text.Element.ALIGN_JUSTIFIED_ALL;
                logoImage.SetAbsolutePosition(60f, 685f);
                logoImage.ScaleAbsolute(150, 132);
                doc.Add(logoImage);
                logoImage = null;
            }
           
            

            //Item Names row
            iTextSharp.text.Font fontTable = FontFactory.GetFont("HELVETICA", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            PdfPTable table = new PdfPTable(noOfInfoColumns + 1);
            PdfPCell cell = new PdfPCell(new Phrase("#", fontTable));
            cell.BorderColor = new BaseColor(37, 37, 37);
            cell.BackgroundColor = new BaseColor(233, 244, 249);
            cell.MinimumHeight = 20;
            cell.BorderWidthTop = 0.25f;
            cell.BorderWidthLeft = 0.25f;
            cell.BorderWidthBottom = 0.25f;
            cell.BorderWidthRight = 0;
            cell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
            table.AddCell(cell);
            if (status)
                cell = new PdfPCell(new Phrase("Package", fontTable));
            else
                cell = new PdfPCell(new Phrase("Therapist", fontTable));
            cell.BorderColor = new BaseColor(37, 37, 37);
            cell.BackgroundColor = new BaseColor(233, 244, 249);
            cell.MinimumHeight = 20;
            cell.BorderWidthTop = 0.25f;
            cell.BorderWidthLeft = 0.25f;
            cell.BorderWidthBottom = 0.25f;
            cell.BorderWidthRight = 0;
            cell.HorizontalAlignment = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Description", fontTable));
            cell.BorderColor = new BaseColor(37, 37, 37);
            cell.BackgroundColor = new BaseColor(233, 244, 249);
            cell.MinimumHeight = 20;
            cell.BorderWidthTop = 0.25f;
            cell.BorderWidthLeft = 0.25f;
            cell.BorderWidthBottom = 0.25f;
            cell.BorderWidthRight = 0;
            cell.HorizontalAlignment = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Unit Price", fontTable));
            cell.BorderColor = new BaseColor(37, 37, 37);
            cell.BackgroundColor = new BaseColor(233, 244, 249);
            cell.MinimumHeight = 20;
            cell.BorderWidthTop = 0.25f;
            cell.BorderWidthLeft = 0.25f;
            cell.BorderWidthBottom = 0.25f;
            cell.BorderWidthRight = 0;
            cell.HorizontalAlignment = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Discount", fontTable));
            cell.BorderColor = new BaseColor(37, 37, 37);
            cell.BackgroundColor = new BaseColor(233, 244, 249);
            cell.MinimumHeight = 20;
            cell.BorderWidthTop = 0.25f;
            cell.BorderWidthLeft = 0.25f;
            cell.BorderWidthBottom = 0.25f;
            cell.BorderWidthRight = 0;
            cell.HorizontalAlignment = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Line Total", fontTable));
            cell.BorderColor = new BaseColor(37, 37, 37);
            cell.BackgroundColor = new BaseColor(233, 244, 249);
            cell.MinimumHeight = 20;
            cell.BorderWidthTop = 0.25f;
            cell.BorderWidthLeft = 0.25f;
            cell.BorderWidthBottom = 0.25f;
            cell.BorderWidthRight = 0.25f;
            cell.HorizontalAlignment = 0;
            table.AddCell(cell);

            //grid values drawing

            bool isLastCol = false;

            int totalValidItemCount = sales_Info.Count;
            int printedRowCount = 0;

            for (int eachItem = 0; eachItem < totalValidItemCount; eachItem++)
            {
                if((eachItem % (noOfInfoColumns + 1)) == 0)
                {
                    if ((sales_Info[eachItem + 1] == "0") && (sales_Info[eachItem + 3] == "0")
                        && (sales_Info[eachItem + 4] == "0") && (sales_Info[eachItem + 5] == "0"))
                    {
                        eachItem += 4;
                        continue;
                    }
                }

                if ((eachItem % (noOfInfoColumns + 1)) == noOfInfoColumns)
                    isLastCol = true;
                else 
                    isLastCol = false;

                if ((eachItem % (noOfInfoColumns + 1)) == 0)
                {
                    printedRowCount++;
                    cell = new PdfPCell(new Phrase(printedRowCount.ToString(), fontTable));
                }
                else
                    cell = new PdfPCell(new Phrase(sales_Info[eachItem], fontTable));

                cell.BorderColor = new BaseColor(37, 37, 37);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_JUSTIFIED_ALL;
                cell.BorderWidthLeft = 0.25f;
                cell.BorderWidthBottom = 0.25f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthRight = 0;
                if (isLastCol)
                    cell.BorderWidthRight = 0.25f;
                cell.MinimumHeight = 30;
                table.AddCell(cell);
            }
            
            cell = new PdfPCell(new Phrase("", fontTable));
            cell.BorderWidth = 0;
            cell.MinimumHeight = 30;
            table.AddCell(cell);
            table.AddCell(cell);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Total", fontTable));
            cell.MinimumHeight = 30;
            cell.BorderColor = new BaseColor(37, 37, 37);
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.BorderWidth = 0;

            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("", fontTable));
            cell.MinimumHeight = 30;
            cell.BorderWidthBottom = 0.25f;
            cell.BorderWidthRight = 0;
            cell.BorderWidthTop = 0;
            cell.BorderWidthLeft = 0.25f;
            cell.BorderColor = new BaseColor(37, 37, 37);
            cell.BorderWidth = 0.25f;

            table.AddCell(cell);
            //int retval = calculateSum();
            cell = new PdfPCell(new Phrase(cust_Info[10], fontTable));
            cell.MinimumHeight = 30;
            cell.BorderColor = new BaseColor(37, 37, 37);
            cell.BorderWidthBottom = 0.25f;
            cell.BorderWidthRight = 0.25f;
            cell.BorderWidthTop = 0;
            cell.BorderWidthLeft = 0.25f;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("", fontTable));
            cell.BorderWidth = 0;
            cell.MinimumHeight = 30;
            table.AddCell(cell);
            table.AddCell(cell);
            table.AddCell(cell);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Advance Recvd.", fontTable));
            cell.MinimumHeight = 30;
            cell.BorderColor = new BaseColor(37, 37, 37);
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.BorderWidth = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(cust_Info[11], fontTable));
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0.25f;
            cell.BorderWidthLeft = 0.25f;
            cell.BorderWidthBottom = 0.25f;
            cell.MinimumHeight = 30;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.BorderColor = new BaseColor(37, 37, 37);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("", fontTable));
            cell.BorderWidth = 0;
            cell.MinimumHeight = 30;
            table.AddCell(cell);
            table.AddCell(cell);
            table.AddCell(cell);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Total Payable", fontTable));
            cell.MinimumHeight = 30;
            cell.BorderColor = new BaseColor(37, 37, 37);
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderWidth = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(cust_Info[12], fontTable));
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0.25f;
            cell.BorderWidthLeft = 0.25f;
            cell.BorderWidthBottom = 0.25f;
            cell.MinimumHeight = 30;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.BorderColor = new BaseColor(37, 37, 37);
            table.AddCell(cell);

            table.SetTotalWidth(new float[] { 0.4F, 1.2F, 4.0F, 1.0F, 1.0F, 1.0F });
            table.TotalWidth = 468;
            table.WriteSelectedRows(0, -1, 60, 550, cb);

        }

        /*public static int calculateSum()
        {
            int sumOfSales = 0;
            int gridVal = 0;
            for (int count = 0; count < sales_Info.Count; count++)
            {
                if (count % (noOfColumns+1) == noOfColumns)
                {
                    gridVal = Int32.Parse(sales_Info[count]);
                    sumOfSales += gridVal;
                }
            }
            return sumOfSales;
        }*/

        public static int i;
    }
}
