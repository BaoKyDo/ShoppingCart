using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2
{
    public class Invoice
    {
        //declare fields
        private int invoiceID;
        private DateTime invoiceDate;
        private bool ship;
        private string customerName;
        private string customerAddress;
        private string customerEmail;

        //default constructor
        public Invoice() { }

        //constructor
        public Invoice(int InvoiceID, DateTime InvoiceDate, bool Ship, string CustomerName, string CustomerAddress, string CustomerEmail)
        {
            this.InvoiceID = InvoiceID;
            this.InvoiceDate = InvoiceDate;
            this.Ship = Ship;
            this.CustomerName = CustomerName;
            this.CustomerAddress = CustomerAddress;
            this.CustomerEmail = CustomerEmail;
        }

        //SETTER AND GETTER METHODS
        public int InvoiceID { get => invoiceID; set => invoiceID = value; }
        public DateTime InvoiceDate { get => invoiceDate; set => invoiceDate = value; }
        public bool Ship { get => ship; set => ship = value; }
        public string CustomerName { get => customerName; set => customerName = value; }
        public string CustomerAddress { get => customerAddress; set => customerAddress = value; }
        public string CustomerEmail { get => customerEmail; set => customerEmail = value; }

        //override ToString Method
        public override string ToString()
        {
            string s = $"{InvoiceID, -35} {CustomerName, -35} {CustomerEmail,-35} {Ship}";
            return s;
        }

        //override Equals and HashCodes methods
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (this.InvoiceID == ((Invoice)obj).InvoiceID)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        public override int GetHashCode()
        {
            return this.InvoiceID;
        }

    }
}
