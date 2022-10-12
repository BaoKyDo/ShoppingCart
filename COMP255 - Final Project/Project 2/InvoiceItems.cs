using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2
{
    class InvoiceItems
    {
        //declare fields
        private int itemID;
        private int invoiceID;
        private string itemName;
        private string itemDescription;
        private decimal itemPrice;
        private int itemQuantity;
        private decimal price;
        
        //default constructor
        public InvoiceItems() { }
        
        //constructor
        public InvoiceItems(int ItemID, int InvoiceID, string ItemName, string ItemDescription, decimal ItemPrice, int ItemQuantity)
        {
            this.ItemID = ItemID;
            this.invoiceID = InvoiceID;
            this.ItemName = ItemName;
            this.ItemDescription = ItemDescription;
            this.ItemPrice = ItemPrice;
            this.ItemQuantity = ItemQuantity;

        }

        //SETTER AND GETTER METHODS
        public int ItemID { get => itemID; set => itemID = value; }
        public int InvoiceID { get => invoiceID; set => invoiceID = value; }
        public string ItemName { get => itemName; set => itemName = value; }
        public string ItemDescription { get => itemDescription; set => itemDescription = value; }
        public decimal ItemPrice { get => itemPrice; set => itemPrice = value; }
        public int ItemQuantity { get => itemQuantity; set => itemQuantity = value; }
        public decimal Price { get => price; set => price = value; }

        //override ToString method
        public override string ToString()
        {
            string s = $"{ItemID, -15} {ItemName, -20} {ItemDescription, -35} {ItemPrice,-25} {ItemQuantity, -15} {Price}";
            return s;
        }

        //overide Equals and GetHashCode()
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (this.ItemID == ((InvoiceItems)obj).ItemID)
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
            return this.ItemID;
        }

    }
}
