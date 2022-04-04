using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace Crystals.Controllers
{
    class XmlOperations
    {
        public XmlOperations(string filename, string invoiceNo, List<string> custInfo, List<string> salesInfo)
        {
            XmlTextWriter writer = new XmlTextWriter(System.IO.Directory.GetCurrentDirectory() + filename, System.Text.Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 1;
            writer.IndentChar = '\t';
            writer.WriteStartElement("InvoiceData");
            createCustomerNode(custInfo[6], custInfo[1], custInfo[3], custInfo[4], custInfo[5], custInfo[2], writer);//CustomerInfo
            createInvoiceNode(invoiceNo, custInfo[0], writer);//InvoiceInfo
            createSalesNode(custInfo[7], custInfo[8], writer);
            createInvoiceMaterials(salesInfo, writer);
            createTotalNode(custInfo[10], custInfo[11], custInfo[12], writer);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();

            //Process.Start(System.IO.Directory.GetCurrentDirectory() + filename);
            //MessageBox.Show("XML File created ! ");
        }
        
        private void createCustomerNode(string cID, string cName, string cPerson, string cNumber, string eMail, string cAddr, XmlTextWriter writer)
        {
            writer.WriteStartElement("CustomerInfo");
            writer.WriteStartElement("CustomerID");
            writer.WriteString(cID);
            writer.WriteEndElement();
            writer.WriteStartElement("Company");
            writer.WriteString(cName);
            writer.WriteEndElement();
            writer.WriteStartElement("ContactPerson");
            writer.WriteString(cPerson);
            writer.WriteEndElement();
            writer.WriteStartElement("Phone");
            writer.WriteString(cNumber);
            writer.WriteEndElement();
            writer.WriteStartElement("Email");
            writer.WriteString(eMail);
            writer.WriteEndElement();
            writer.WriteStartElement("Address");
            string replacement = Regex.Replace(cAddr, @"\n|\r", " ");
            writer.WriteString(replacement);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
        
        private void createInvoiceNode(string invNo, string invDate, XmlTextWriter writer)
        {
            writer.WriteStartElement("InvoiceInfo");
            writer.WriteStartElement("Number");
            writer.WriteString(invNo);
            writer.WriteFullEndElement();
            writer.WriteStartElement("Date");
            writer.WriteString(invDate);
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
        }

        private void createSalesNode(string empPost, string empName, XmlTextWriter writer)
        {
            writer.WriteStartElement("SalesTeam");
            writer.WriteStartElement("EmpPost");
            writer.WriteString(empPost);
            writer.WriteEndElement();
            writer.WriteStartElement("EmpName");
            writer.WriteString(empName);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private void createTotalNode(string sTotal, string sAdvance, string sPayable, XmlTextWriter writer)
        {
            writer.WriteStartElement("Payment");
            writer.WriteStartElement("TotalAmount");
            writer.WriteString(sTotal);
            writer.WriteEndElement();
            writer.WriteStartElement("AdvancePaid");
            writer.WriteString(sAdvance);
            writer.WriteEndElement();
            writer.WriteStartElement("Payable");
            writer.WriteString(sPayable);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private void createInvoiceMaterials(List<string> salesInfo, XmlTextWriter writer)
        {
            writer.WriteStartElement("InvoiceMaterials");
            int totalValidItemCount = salesInfo.Count;
            for (int eachItem = 0; eachItem < totalValidItemCount; eachItem= eachItem+6)
            {

                writer.WriteStartElement("Item");
                writer.WriteAttributeString("id", salesInfo[eachItem]); 
                writer.WriteStartElement("Qty");
                writer.WriteString(salesInfo[eachItem+1]);
                writer.WriteEndElement();
                writer.WriteStartElement("Desc");
                writer.WriteString(salesInfo[eachItem + 2]);
                writer.WriteEndElement();
                writer.WriteStartElement("UnitPrice");
                writer.WriteString(salesInfo[eachItem + 3]);
                writer.WriteEndElement();
                writer.WriteStartElement("Discount");
                writer.WriteString(salesInfo[eachItem + 4]);
                writer.WriteEndElement();
                writer.WriteStartElement("LineTotal");
                writer.WriteString(salesInfo[eachItem + 5]);
                writer.WriteEndElement();
                writer.WriteEndElement();

            }
            writer.WriteEndElement();
        }

    }
}
