using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace Final_Project_DB_Lab
{
    public partial class inventoryForm : UserControl
    {
        string conStr = @"DATA SOURCE = localhost:1521/XE; USER ID=F219088;PASSWORD=5736";
        OracleConnection connection;
        string query;
        public inventoryForm()
        {
            InitializeComponent();
            setProAmount();
            setCategoryGrid();
            query = "SELECT item_name Name, item_quantity Quantity, item_price Price, item_barcode Barcode, rack_no Rack, item_desc Description, reorder_point Reorder FROM item";
            setProdGrid(query);
        }
        private void resetFields()
        {
            nametxt.Text = "";
            pricetxt.Text = "";
            reordertxt.Text = "";
            descriptxt.Text = "";
            barcodetxt.Text = "";
            quantitytxt.Text = "";
            racknotxt.Text = "";
        }
        private void setProdGrid(string query)
        {
            connection = new OracleConnection(conStr);
            connection.Open();
            using (OracleCommand command = new OracleCommand(query, connection))
            {
                OracleDataAdapter adapter = new OracleDataAdapter();
                adapter.SelectCommand = command;
                DataTable table = new DataTable();
                adapter.Fill(table);
                prodGrid.DataSource = table;
            }
            connection.Close();
        }
        private void setProAmount()
        {
            connection = new OracleConnection(conStr);
            connection.Open();
            query = "SELECT SUM(item_quantity) FROM item";
            using (OracleCommand command = new OracleCommand(query, connection))
            {
                this.totalProdLabel.Text = Convert.ToString(command.ExecuteScalar());
                if (this.totalProdLabel.Text == "")
                {
                    this.totalProdLabel.Text = "0";
                }
            }
            query = "SELECT SUM(item_quantity * item_price) FROM item";
            using (OracleCommand command = new OracleCommand(query, connection))
            {
                this.totalAmountLabel.Text = Convert.ToString(command.ExecuteScalar()) + " $";
                if (this.totalAmountLabel.Text == " $")
                {
                    this.totalAmountLabel.Text = "0.0 $";
                }
            }
            connection.Close();
        }
        private void setCategoryGrid()
        {
            connection = new OracleConnection(conStr);
            connection.Open();
            query = "SELECT cat_name Name, cat_description Description FROM category";
            using (OracleCommand command = new OracleCommand(query, connection))
            {
                OracleDataAdapter adapter = new OracleDataAdapter();
                adapter.SelectCommand = command;
                DataTable table = new DataTable();
                adapter.Fill(table);
                categoriesGrid.DataSource = table;
            }
            connection.Close();
        }

        private void receiptNotxt_Click(object sender, EventArgs e)
        {

        }

        private void addbtn_Enter(object sender, EventArgs e)
        {

        }

        private void addbtn_MouseHover(object sender, EventArgs e)
        {

        }

        private void searchbtn_Click(object sender, EventArgs e)
        {
            if(this.prodIDtxt.Text != "")
            {
                if (searchbyid.Checked)
                {
                    string query = "SELECT item_name Name, item_quantity Quantity, item_price Price, item_barcode Barcode, rack_no Rack, item_desc Description, reorder_point Reorder FROM item WHERE item_id = " + this.prodIDtxt.Text;
                    setProdGrid(query);
                }
                else if (searchbyname.Checked)
                {
                    string query = "SELECT item_name Name, item_quantity Quantity, item_price Price, item_barcode Barcode, rack_no Rack, item_desc Description, reorder_point Reorder FROM item WHERE item_name = '" + this.prodIDtxt.Text + "'";
                    setProdGrid(query);
                }
                else
                {
                    string query = "SELECT item_name Name, item_quantity Quantity, item_price Price, item_barcode Barcode, rack_no Rack, item_desc Description, reorder_point Reorder FROM item WHERE cat_name = '" + this.prodIDtxt.Text + "'";
                    setProdGrid(query);
                }
            }
        }
        private string checkRackExist()
        {
            connection = new OracleConnection(conStr);
            connection.Open();
            string query = "SELECT cat_name FROM rack WHERE rack_no = " + this.racknotxt.Text;
            string cat_name = "";
            using (OracleCommand command = new OracleCommand(query, connection))
            {
                cat_name = Convert.ToString(command.ExecuteScalar());
                if (cat_name == "")
                {
                    MessageBox.Show("Rack not found. Please make the rack first.");
                }
            }
            connection.Close();
            return cat_name;
        }

        private void addbtn_Click(object sender, EventArgs e)
        {
            if(this.nametxt.Text == "" || !this.pricetxt.MaskCompleted || !this.reordertxt.MaskCompleted || !this.barcodetxt.MaskCompleted || !this.quantitytxt.MaskCompleted || !this.racknotxt.MaskCompleted)
            {
                MessageBox.Show("Please fill all the fields.");
            }
            else
            {
                string cat_name = checkRackExist();
                if(cat_name != "")
                {
                    connection = new OracleConnection(conStr);
                    connection.Open();
                    query = "INSERT INTO item VALUES(item_id_SEQ.NEXTVAL,'" + this.nametxt.Text + "'," + this.quantitytxt.Text + "," + this.pricetxt.Text + "," + this.reordertxt.Text + ",'" + this.descriptxt.Text + "'," + this.barcodetxt.Text + ",'" + cat_name + "'," + this.racknotxt.Text + ")";
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        try
                        {
                            command.ExecuteNonQuery();
                            query = "SELECT item_name Name, item_quantity Quantity, item_price Price, item_barcode Barcode, rack_no Rack, item_desc Description, reorder_point Reorder FROM item";
                            setProdGrid(query);
                            setProAmount();
                            resetFields();
                        }
                        catch
                        {
                            MessageBox.Show("Item with same barcode already exists.");
                        }
                    }
                    connection.Close();
                }
            }
        }

        private void deletebtn_Click(object sender, EventArgs e)
        {
            if (!this.barcodetxt.MaskCompleted)
            {
                MessageBox.Show("Please select an item to delete.");
            }
            else
            {
                connection = new OracleConnection(conStr);
                connection.Open();
                string query = "DELETE FROM item WHERE item_barcode = " + this.barcodetxt.Text;
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    try
                    {
                        int rows = command.ExecuteNonQuery();
                        if (rows <= 0)
                        {
                            MessageBox.Show("Item not found.");
                        }
                        else
                        {
                            query = "SELECT item_name Name, item_quantity Quantity, item_price Price, item_barcode Barcode, rack_no Rack, item_desc Description, reorder_point Reorder FROM item";
                            setProdGrid(query);
                            setProAmount();
                            resetFields();
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Item cannot be deleted.");
                    }
                }
                connection.Close();
            }
        }

        private void updatebtn_Click(object sender, EventArgs e)
        {
            if(!this.barcodetxt.MaskCompleted)
            {
                MessageBox.Show("Please select an item to update.");
            }
            else
            {
                connection = new OracleConnection(conStr);
                connection.Open();
                string query = "SELECT item_barcode FROM item WHERE item_barcode = " + this.barcodetxt.Text;
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    if(command.ExecuteScalar() == null)
                    {
                        MessageBox.Show("Item not found");
                        return;
                    }
                }
                if (this.nametxt.Text != "")
                {
                    query = "UPDATE item SET item_name = '" + this.nametxt.Text + "' WHERE item_barcode = " + this.barcodetxt.Text;
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        query = "SELECT item_name Name, item_quantity Quantity, item_price Price, item_barcode Barcode, rack_no Rack, item_desc Description, reorder_point Reorder FROM item";
                        setProdGrid(query);
                        setProAmount();
                    }
                }
                connection.Close();
                connection = new OracleConnection(conStr);
                connection.Open();
                if (this.quantitytxt.MaskCompleted)
                {
                    connection = new OracleConnection(conStr);
                    connection.Open();
                    query = "UPDATE item SET item_quantity = " + this.quantitytxt.Text + " WHERE item_barcode = " + this.barcodetxt.Text;
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        query = "SELECT item_name Name, item_quantity Quantity, item_price Price, item_barcode Barcode, rack_no Rack, item_desc Description, reorder_point Reorder FROM item";
                        setProdGrid(query);
                        setProAmount();
                    }
                    connection.Close();
                }
                if (this.pricetxt.MaskCompleted)
                {
                    connection = new OracleConnection(conStr);
                    connection.Open();
                    query = "UPDATE item SET item_price = " + this.pricetxt.Text + " WHERE item_barcode = " + this.barcodetxt.Text;
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        query = "SELECT item_name Name, item_quantity Quantity, item_price Price, item_barcode Barcode, rack_no Rack, item_desc Description, reorder_point Reorder FROM item";
                        setProdGrid(query);
                        setProAmount();
                    }
                    connection.Close();
                }
                if (reordertxt.MaskCompleted)
                {
                    connection = new OracleConnection(conStr);
                    connection.Open();
                    query = "UPDATE item SET reorder_point = " + this.reordertxt.Text + " WHERE item_barcode = " + this.barcodetxt.Text;
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        query = "SELECT item_name Name, item_quantity Quantity, item_price Price, item_barcode Barcode, rack_no Rack, item_desc Description, reorder_point Reorder FROM item";
                        setProdGrid(query);
                        setProAmount();
                    }
                    connection.Close();
                }
                if (descriptxt.Text != "")
                {
                    connection = new OracleConnection(conStr);
                    connection.Open();
                    query = "UPDATE item SET item_desc = '" + this.descriptxt.Text + "' WHERE item_barcode = " + this.barcodetxt.Text;
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        query = "SELECT item_name Name, item_quantity Quantity, item_price Price, item_barcode Barcode, rack_no Rack, item_desc Description, reorder_point Reorder FROM item";
                        setProdGrid(query);
                        setProAmount();
                    }
                    connection.Close();
                }
                if (racknotxt.MaskCompleted)
                {
                    string cat_name = checkRackExist();
                    if (cat_name != "")
                    {
                        query = "UPDATE item SET rack_no = " + this.racknotxt.Text + " WHERE item_barcode = " + this.barcodetxt.Text;
                        connection = new OracleConnection(conStr);
                        connection.Open();
                        using (OracleCommand command = new OracleCommand(query, connection))
                        {
                            command.ExecuteNonQuery();
                            query = "SELECT item_name Name, item_quantity Quantity, item_price Price, item_barcode Barcode, rack_no Rack, item_desc Description, reorder_point Reorder FROM item";
                            setProdGrid(query);
                            setProAmount();
                        }
                        connection.Close();
                        connection = new OracleConnection(conStr);
                        connection.Open();
                        query = "UPDATE item SET cat_name = '" + cat_name + "' WHERE item_barcode = " + this.barcodetxt.Text;
                        using (OracleCommand command = new OracleCommand(query, connection))
                        {
                            command.ExecuteNonQuery();
                            query = "SELECT item_name Name, item_quantity Quantity, item_price Price, item_barcode Barcode, rack_no Rack, item_desc Description, reorder_point Reorder FROM item";
                            setProdGrid(query);
                            setProAmount();
                        }
                        connection.Close();
                    }
                }
                resetFields();
            }
        }

        private void inventoryForm_VisibleChanged(object sender, EventArgs e)
        {
            resetFields();
            setProAmount();
            setCategoryGrid();
            string query = "SELECT item_name Name, item_quantity Quantity, item_price Price, item_barcode Barcode, rack_no Rack, item_desc Description, reorder_point Reorder FROM item";
            setProdGrid(query);
        }

        private void prodGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is in the row header or not
            if (e.RowIndex >= 0)
            {
                // Access the clicked row using the RowIndex property
                DataGridViewRow clickedRow = prodGrid.Rows[e.RowIndex];

                // Retrieve the data from the desired cells in the clicked row
                nametxt.Text = clickedRow.Cells[0].Value.ToString();  // Assuming the first column
                quantitytxt.Text = clickedRow.Cells[1].Value.ToString();  // Assuming the second column
                pricetxt.Text = clickedRow.Cells[2].Value.ToString();  // Assuming the second column
                barcodetxt.Text = clickedRow.Cells[3].Value.ToString();  // Assuming the second column
                racknotxt.Text = clickedRow.Cells[4].Value.ToString();  // Assuming the second column
                descriptxt.Text = clickedRow.Cells[5].Value.ToString();  // Assuming the second colum
                reordertxt.Text = clickedRow.Cells[6].Value.ToString();  // Assuming the second colum
            }
        }

        private void prodIDtxt_TextChanged(object sender, EventArgs e)
        {

        }

        private void searchbyid_CheckedChanged(object sender, EventArgs e)
        {
            if (searchbyid.Checked)
            {
                searchbyname.Checked = false;
                searchbycategory.Checked = false;
            }
        }

        private void searchbyname_CheckedChanged(object sender, EventArgs e)
        {
            if (searchbyname.Checked)
            {
                searchbycategory.Checked = false;
                searchbyid.Checked = false;
            }
        }

        private void searchbycategory_CheckedChanged(object sender, EventArgs e)
        {
            if (searchbycategory.Checked)
            {
                searchbyid.Checked = false;
                searchbyname.Checked = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShortListItemsForm obj = new ShortListItemsForm();
            obj.ShowDialog();
        }
    }
}
