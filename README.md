# ShoppingCart
 The application will have 2 tables, one for Invoices (one record per invoice) and a related table for the list of items for each invoice. Allow a user to view, add new records, and update or delete existing records from the database

GENERAL FUNCTIONALITY:
a,When the user selects an Invoice row in the Invoices Listbox at the top of the form:
i. the details for that Invoice should appear in the Invoice Record form fields below.
ii. a full list of the Invoice Items for that Invoice should appear in the Invoice Items Listbox.
b. When the user selects an Invoice Item record in the Invoice Items Listbox, the details of
that Invoice Item record should appear in the form fields at the bottom of the form.
c. The Subtotal, PST, GST and Total Amounts should be calculated and shown in the
appropriate textboxes below the Invoice Items listbox. The user should not enter data in
these textboxes - these values will be managed by your code. (Set the IsReadOnly
property to true on these textboxes)
d. The SAVE / DELETE / NEW buttons should allow the user to modify or add new Invoice
records and Invoice Item records.
