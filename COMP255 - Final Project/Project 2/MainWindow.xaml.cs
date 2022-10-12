using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // declare variables and objects
        private List<Invoice> InvoiceList = new List<Invoice>();
        private List<InvoiceItems> ItemList = new List<InvoiceItems>();
        private Invoice CurrentInvoiceRecord;
        private int CurrentInvoiceIndex;
        private InvoiceItems CurrentItem;
        private int CurrentItemIndex;
        private decimal PST;
        private decimal GST;
        private decimal Total;
        private decimal Subtotal;


        public MainWindow()
        {
            InitializeComponent();

            // recall Load method to display invoice at first
            LoadInvoices();

        }

        // load invoice method
        void LoadInvoices()
        {

            //Clear the List and ListBox 
            InvoiceRecordListBox.Items.Clear();
            InvoiceList.Clear();

            
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =
                    @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|ShoppingCart.mdf;Integrated Security=True";

                // open connection for database
                connection.Open();

                string sql = "SELECT * FROM Invoices;";

                SqlCommand myCommand = new SqlCommand(sql, connection);


                // Loop over and read the SQLDataReader 
                using (SqlDataReader Reader = myCommand.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        // creat invoice obj for each record
                        Invoice NewInvoiceRecord = new Invoice((int)Reader["InvoiceID"],
                                                              (DateTime)Reader["InvoiceDate"],
                                                              (bool)Reader["Shipped"],
                                                              (string)Reader["CustomerName"],
                                                              (string)Reader["CustomerAddress"],
                                                              (string)Reader["CustomerEmail"]);

                        InvoiceList.Add(NewInvoiceRecord);
                        InvoiceRecordListBox.Items.Add(NewInvoiceRecord);

                    }
                    connection.Close();
                }

            }
        }

        //Load items method
        void LoadItems()
        {
            // clear list and listbox
            InvoiceItemListBox.Items.Clear();
            ItemList.Clear();

            // set subtotal to 0 so that each customer has different subtotal
            // invoice increasing subtotal when select another new customer
            Subtotal = 0;

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =
                    @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|ShoppingCart.mdf;Integrated Security=True";

                //open connection
                connection.Open();

                string sql = "SELECT * FROM InvoiceItems " +
                    $"WHERE InvoiceID = {$"(SELECT InvoiceID FROM Invoices WHERE InvoiceID = {CurrentInvoiceRecord.InvoiceID})"}";

                SqlCommand myCommand = new SqlCommand(sql, connection);

                //loop through to load SQL DateReader with all elements in Invoice Items data
                //that match the InvoiceID from Invoices table data
                using (SqlDataReader Reader = myCommand.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        InvoiceItems NewItem = new InvoiceItems((int)Reader["ItemID"],
                                                                (int)Reader["InvoiceID"],
                                                                (string)Reader["ItemName"],
                                                                (string)Reader["ItemDescription"],
                                                                (decimal)Reader["ItemPrice"],
                                                                (int)Reader["ItemQuantity"]);

                        //calculate price and subtotal
                        NewItem.Price = NewItem.ItemQuantity * NewItem.ItemPrice;
                        Subtotal += NewItem.Price;

                        // add new item to a list and listbox
                        InvoiceItemListBox.Items.Add(NewItem);
                        ItemList.Add(NewItem);
                    }

                    //destroy connection
                    connection.Close();
                }

            }
        }

        // Display PST, GST, Subtotal, Total Method
        void DisplayCost()
        {
            // when item got selected then Costs to the textboxes
            if (CurrentItem != null)
            {
                PST = Subtotal * (decimal)0.06;
                GST = Subtotal * (decimal)0.05;
                Total = Subtotal + PST + GST;

                SubtotalTextBox.Text = string.Format("{0:0.00}", Subtotal);
                PSTTextBox.Text = string.Format("{0:0.00}", PST);
                GSTTextBox.Text = string.Format("{0:0.00}", GST);
                TotalTextBox.Text = string.Format("{0:0.00}", Total);
            }
            // when no item selected -> Clear all
            else
            {
                SubtotalTextBox.Clear();
                PSTTextBox.Clear();
                GSTTextBox.Clear();
                TotalTextBox.Clear();
            }
        }

        // Invoice record Listbox
        private void InvoiceRecordListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // point to the record and its index that get seleced on the listbox
            CurrentInvoiceRecord = (Invoice)InvoiceRecordListBox.SelectedItem;
            CurrentInvoiceIndex = InvoiceRecordListBox.SelectedIndex;


            //call display invoice method
            DisplayInvoiceRecords(CurrentInvoiceIndex);

            // item get selected then load Item
            //no item selected then clear list and listbox
            if (CurrentInvoiceIndex != -1)
            {
                LoadItems();
            }
            else
            {
                InvoiceItemListBox.Items.Clear();
                ItemList.Clear();
            }



        }

        //Output the current Invoice record to the textboxs
        private void DisplayInvoiceRecords(int CurrentRecord)
        {
            // if data is good -> Record is selected -> Display data out to the proper textboxes
            if (CurrentRecord != -1)
            {
                InvoiceIDTextBox.Text = InvoiceList[CurrentRecord].InvoiceID.ToString();
                InvoiceDateTextBox.Text = InvoiceList[CurrentRecord].InvoiceDate.ToString();
                if (InvoiceList[CurrentRecord].Ship == true)
                {
                    ShipCheckBox.IsChecked = true;
                }
                else
                {
                    ShipCheckBox.IsChecked = false;
                }
                CustomerNameTextBox.Text = InvoiceList[CurrentRecord].CustomerName;
                CustomerAddressTextBox.Text = InvoiceList[CurrentRecord].CustomerAddress;
                CustomerEmailTextBox.Text = InvoiceList[CurrentRecord].CustomerEmail;

            }
            // if data fail -> Clear all the textboxes
            else
            {

                InvoiceIDTextBox.Clear();
                InvoiceDateTextBox.Clear();
                ShipCheckBox.IsChecked = false;
                CustomerNameTextBox.Clear();
                CustomerAddressTextBox.Clear();
                CustomerEmailTextBox.Clear();
            }



        }

        //Item ListBox
        private void InvoiceItemListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // point to the item and its index
            CurrentItem = (InvoiceItems)InvoiceItemListBox.SelectedItem;
            CurrentItemIndex = InvoiceItemListBox.SelectedIndex;

            // display item data to the textboxes
            DisplayItems(CurrentItem);

            // display PST, GST, Total and subtotal in the textboxes
            DisplayCost();

        }

        // Display Item infor into the proper textboxes
        private void DisplayItems(InvoiceItems CurrentItem)
        {
            //data is fine -> display
            if (CurrentItem != null)
            {
                ItemIDTextBox.Text = CurrentItem.ItemID.ToString();
                ItemNameTextBox.Text = CurrentItem.ItemName;
                ItemDescriptionTextBox.Text = CurrentItem.ItemDescription;
                ItemPriceTextBox.Text = CurrentItem.ItemPrice.ToString();
                ItemQuantityTextBox.Text = CurrentItem.ItemQuantity.ToString();

            }
            else
            //data fails -> Clear all
            {
                ItemIDTextBox.Clear();
                ItemNameTextBox.Clear();
                ItemDescriptionTextBox.Clear();
                ItemPriceTextBox.Clear();
                ItemQuantityTextBox.Clear();

            }
        }

        //IsDataValid function to check for the validation of the text boxes for the InvoiceRecord only 
        private bool IsDataValid()
        {
            //textboxes fail -> print out error message
            // and return false
            if (CustomerNameTextBox.Text == "")
            {

                ErrorMessageLabel.Content = "Please fill in your name";
                return false;

            }
            else if (CustomerNameTextBox.Text == "")
            {
                ErrorMessageLabel.Content = "Please fill in your email";
                return false;
            }
            else if (InvoiceDateTextBox.Text == "")
            {
                ErrorMessageLabel.Content = "Please fill in Invoice Date";
                return false;
            }

            else
            {

                return true;

            }
        }

        //IsItemDataValid checks fot the validation of the textboxes for the Invoice Item only
        private bool IsItemDataValid()
        {
            //textbox is invalid -> printout error message
            // and return false
            // otherwise return true
            if (ItemNameTextBox.Text == "")
            {
                ErrorMessageLabel.Content = "Please fill in item name";
                return false;
            }
            else if (ItemPriceTextBox.Text == "" || Convert.ToDecimal(ItemPriceTextBox.Text) < 0)
            {
                ErrorMessageLabel.Content = "Please fill in item price and price can not be negative";
                return false;
            }
            else if (ItemQuantityTextBox.Text == "" || Convert.ToInt32(ItemQuantityTextBox.Text) < 0)
            {
                ErrorMessageLabel.Content = "Please fill in item quantity and quantity can not be negative";
                return false;
            }
            else
            {

                return true;

            }


        }


        // Save Invoice Record Method
        private void SaveInvoicesButton_Click(object sender, RoutedEventArgs e)
        {
            // check for qualified value of the Invoice Record
            if (IsDataValid() == false) return;

            // data is good
            // current index points to the item that is selected on the Invoice Record List box
            int CurrentIndex = InvoiceRecordListBox.SelectedIndex;

            //get data from the textboxes
            CurrentInvoiceRecord.InvoiceID = Convert.ToInt32(InvoiceIDTextBox.Text);
            CurrentInvoiceRecord.InvoiceDate = Convert.ToDateTime(InvoiceDateTextBox.Text);
            if (ShipCheckBox.IsChecked == true)
            {
                CurrentInvoiceRecord.Ship = true;
            }
            CurrentInvoiceRecord.CustomerName = CustomerNameTextBox.Text;
            CurrentInvoiceRecord.CustomerAddress = CustomerAddressTextBox.Text;
            CurrentInvoiceRecord.CustomerEmail = CustomerEmailTextBox.Text;

            using (SqlConnection connection = new SqlConnection())
            {

                connection.ConnectionString =
                    @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|ShoppingCart.mdf;Integrated Security=True";

                //open connection
                connection.Open();

                // update a new value for existing record
                string sql = "UPDATE Invoices SET " +
                                               $"InvoiceDate = '{CurrentInvoiceRecord.InvoiceDate}'," +
                                               $"Shipped = '{CurrentInvoiceRecord.Ship}'," +
                                               $"CustomerName = '{CurrentInvoiceRecord.CustomerName}'," +
                                               $"CustomerAddress = '{CurrentInvoiceRecord.CustomerAddress}'," +
                                               $"CustomerEmail = '{CurrentInvoiceRecord.CustomerEmail}'" +
                                               $"WHERE InvoiceID = {CurrentInvoiceRecord.InvoiceID};";

                SqlCommand myCommand = new SqlCommand(sql, connection);

                myCommand.ExecuteNonQuery();

                // close connection
                connection.Close();

            }

            // Load new updated record to the listbox
            LoadInvoices();
            //reselect the pre-index
            InvoiceRecordListBox.SelectedIndex = CurrentIndex;

        }

        // Save Existing Item Method
        private void SaveItemButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear any err ocurred before
            ErrorMessageLabel.Content = "";

            //validate both Invoice Record TextBoxes and Item Textboxes
            if (IsDataValid() == false) return;
            if (IsItemDataValid() == false) return;

            // when no current item is selected -> clear all textboxes
            if(CurrentItem == null)
            {
                ItemIDTextBox.Clear();
                ItemNameTextBox.Clear();
                ItemDescriptionTextBox.Clear();
                ItemPriceTextBox.Clear();
                ItemQuantityTextBox.Clear();

                return;
            }

            // data is good 
            //CurrentIndex points to the index of the item is selected in the listbox
            int CurrentIndex = InvoiceItemListBox.SelectedIndex;

            //Receive data from teh textbox
            CurrentItem.ItemID = Convert.ToInt32(ItemIDTextBox.Text);
            CurrentItem.ItemName = ItemNameTextBox.Text;
            CurrentItem.ItemDescription = ItemDescriptionTextBox.Text;
            CurrentItem.ItemPrice = Convert.ToDecimal(ItemPriceTextBox.Text);
            CurrentItem.ItemQuantity = Convert.ToInt32(ItemQuantityTextBox.Text);

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =
                    @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|ShoppingCart.mdf;Integrated Security=True";

                //open connection
                connection.Open();

                // update a new value from the selected Item
                string sql = "UPDATE InvoiceItems SET " +
                                                    $"ItemName = '{CurrentItem.ItemName}'," +
                                                    $"ItemDescription = '{CurrentItem.ItemDescription}'," +
                                                    $"ItemPrice = '{CurrentItem.ItemPrice}'," +
                                                    $"ItemQuantity = '{CurrentItem.ItemQuantity}'" +
                                                    $"WHERE ItemID = {CurrentItem.ItemID};";

                SqlCommand myCommand = new SqlCommand(sql, connection);

                myCommand.ExecuteNonQuery();

                //Close Connection
                connection.Close();
            }

            // reload new updated values and display on the textboxes
            LoadItems();

            // Display PST, GST, total and subtotal
            DisplayCost();

            //reselect the previous index
            InvoiceItemListBox.SelectedIndex = CurrentIndex;


        }

        // Add new record to the List And ListBox
        private void AddNewInvoicesButton_Click(object sender, RoutedEventArgs e)
        {
            //clear any message occured before
            ErrorMessageLabel.Content = "";

            //validate textboxes belongs to Invoice Record 
            if (IsDataValid() == false) return;

            string sql;

            // create a new object to store a new record
            Invoice NewRecord = new Invoice();

            //cannot update or make new InvoiceID
            //NewRecord.InvoiceID = Convert.ToInt32(InvoiceIDTextBox.Text);

            // receive data from textboxes
            NewRecord.InvoiceDate = Convert.ToDateTime(InvoiceDateTextBox.Text);
            if (ShipCheckBox.IsChecked == true)
            {
                NewRecord.Ship = true;
            }
            else
            {
                NewRecord.Ship = false;
            }
            NewRecord.CustomerName = CustomerNameTextBox.Text;
            NewRecord.CustomerAddress = CustomerAddressTextBox.Text;
            NewRecord.CustomerEmail = CustomerEmailTextBox.Text;


            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =
                     @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|ShoppingCart.mdf;Integrated Security=True";

                // open connection
                connection.Open();

                //find and get the maximun ID
                sql = "SELECT MAX(InvoiceID) FROM Invoices;";

                int NewInvoiceID;

                // and increase by one everytime to get a new ID
                using (SqlCommand myCommand = new SqlCommand(sql, connection))
                {
                    NewInvoiceID = Convert.ToInt32(myCommand.ExecuteScalar()) + 1;

                }

                //set InvoiceID to a new ID 
                NewRecord.InvoiceID = NewInvoiceID;

                //Insert all to the Invoices data table
                sql = $"INSERT INTO Invoices " +
                    "(InvoiceID, InvoiceDate, Shipped, CustomerName, CustomerAddress, CustomerEmail) " +
                    "VALUES " +
                    $"({NewRecord.InvoiceID}, " +
                    $"'{NewRecord.InvoiceDate}', " +
                    $"'{NewRecord.Ship}', " +
                    $"'{NewRecord.CustomerName}', " +
                    $"'{NewRecord.CustomerAddress}', " +
                    $"'{NewRecord.CustomerEmail}'); ";

                using (SqlCommand InsertCommand = new SqlCommand(sql, connection))
                {
                    InsertCommand.ExecuteNonQuery();
                }

                //reload all record and new record created
                LoadInvoices();

                //find index of new record
                int NewRecordIndex = InvoiceRecordListBox.Items.IndexOf(NewRecord);

                //points to that new record
                InvoiceRecordListBox.SelectedIndex = NewRecordIndex;

                // close connection
                connection.Close();
            }
        }

        // Add new Item Method
        private void AddNewItemTextBox_Click(object sender, RoutedEventArgs e)
        {
            // clear err message occured before if there is one
            ErrorMessageLabel.Content = "";

            //validate both InvoiceRecord textboxes and InvoiceItem textBoxes
            if (IsDataValid() == false) return;
            if (IsItemDataValid() == false) return;

            string sql;

            //create a new obj to store new Item
            InvoiceItems NewItem = new InvoiceItems();

            // get date from the textboxes
            NewItem.InvoiceID = CurrentInvoiceRecord.InvoiceID;
            NewItem.ItemName = ItemNameTextBox.Text;
            NewItem.ItemDescription = ItemDescriptionTextBox.Text;
            NewItem.ItemPrice = Convert.ToDecimal(ItemPriceTextBox.Text);
            NewItem.ItemQuantity = Convert.ToInt32(ItemQuantityTextBox.Text);

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =
                    @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|ShoppingCart.mdf;Integrated Security=True";

                //open connection
                connection.Open();

                //find and get the maximun Item ID
                sql = "SELECT MAX(ItemID) FROM InvoiceItems";

                int NewItemID;

                //and increase one each time
                using (SqlCommand myCommand = new SqlCommand(sql, connection))
                {
                    NewItemID = Convert.ToInt32(myCommand.ExecuteScalar()) + 1;

                }

                //update Item ID
                NewItem.ItemID = NewItemID;

                // Insert all to the Invoice Item Data table
                sql = $"INSERT INTO InvoiceItems " +
                    "(ItemID, InvoiceID, ItemName, ItemDescription, ItemPrice, ItemQuantity) " +
                    "VALUES " +
                    $"({NewItem.ItemID}, " +
                    $"'{NewItem.InvoiceID}', " +
                    $"'{NewItem.ItemName}', " +
                    $"'{NewItem.ItemDescription}', " +
                    $"'{NewItem.ItemPrice}', " +
                    $"'{NewItem.ItemQuantity}');";


                using (SqlCommand InsertCommand = new SqlCommand(sql, connection))
                {
                    InsertCommand.ExecuteNonQuery();
                }

                // reload to update all the previous items and new item
                LoadItems();

                //find the new Item record
                int NewItemIndex = InvoiceItemListBox.Items.IndexOf(NewItem);

                //reselect pre-Index and display on the list box
                InvoiceItemListBox.SelectedIndex = NewItemIndex;

                //close connection
                connection.Close();
            }
        }

        //Remove Record Method
        private void DeleteInvoicesButton_Click(object sender, RoutedEventArgs e)
        {
            //clear any err message occures
            ErrorMessageLabel.Content = "";

            string sql;

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =
                    @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|ShoppingCart.mdf;Integrated Security=True";

                //open connection
                connection.Open();

                //delete selected record fromt the Invoices data table
                sql = $"DELETE FROM Invoices WHERE InvoiceID = {CurrentInvoiceRecord.InvoiceID};";

                //using try-catch to eliminate any err
                using (SqlCommand myCommand = new SqlCommand(sql, connection))
                {
                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        Exception err = new Exception("No matching record found to be delete", ex);
                        throw err;
                    }
                }

                int IndexToRemove = CurrentInvoiceIndex;

                //remove record from the List
                InvoiceList.Remove(CurrentInvoiceRecord);

                //remove record fromt the lsitbox
                InvoiceRecordListBox.Items.Remove(CurrentInvoiceRecord);

                //wrap up if the selected removed record is the last record
                if (IndexToRemove == InvoiceList.Count)
                {
                    CurrentInvoiceIndex = InvoiceList.Count - 1;
                }
                else
                {
                    CurrentInvoiceIndex = IndexToRemove;
                }

                //reselect the pre-index of the pre-record
                InvoiceRecordListBox.SelectedIndex = CurrentInvoiceIndex;
            }
        }

        //Remove Item Method
        private void DeleteItemTextBox_Click(object sender, RoutedEventArgs e)
        {
            //delete any err messagge occured
            ErrorMessageLabel.Content = "";

            //if no selected item -> print out the err message
            if (IsItemDataValid() == false)
            {
                ErrorMessageLabel.Content = "No item selected to delete";
                return;
            }
            
            string sql;

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =
                    @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|ShoppingCart.mdf;Integrated Security=True";

                //open connection
                connection.Open();

                //delete item from the Invoice Item data table
                sql = $"DELETE FROM InvoiceItems WHERE ItemID = {CurrentItem.ItemID};";

                // use try-catch to eliminate err
                using (SqlCommand myCommand = new SqlCommand(sql, connection))
                {
                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        Exception err = new Exception("No matching record found to be delete", ex);
                        throw err;
                    }
                }

                //Store current selected item index
                int IndexToRemove = CurrentItemIndex;

                //remove item from the item list
                ItemList.Remove(CurrentItem);

                //remove item from the lsit box
                InvoiceItemListBox.Items.Remove(CurrentItem);

                //wrap up if selected item is the last item in the listbox
                if (IndexToRemove == ItemList.Count)
                {
                    CurrentItemIndex = ItemList.Count - 1;
                }
                else
                {
                    CurrentItemIndex = IndexToRemove;
                }

                //reload the table
                LoadItems();

                //output GST, PST, total, subtotal
                DisplayCost();
                
                //reselect pre item index
                InvoiceItemListBox.SelectedIndex = CurrentItemIndex;
                
            }
        }



    }
}
