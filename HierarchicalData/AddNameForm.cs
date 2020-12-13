using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Treeview
{
    public partial class AddNameForm : Form
    {
        public Form1.DataRows.MyRow r = null;
        public TreeNode tn = null;

        public AddNameForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                SqlConnection conn = new SqlConnection(
                                @"Server=192.168.2.110;Integrated Security=true;Database=treeview");
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "insert into datatable (treekey, parentid, data) values("
                    + (r != null ? "'" + r.TreeKey + ".' + " : "")
                    + "cast(ident_current('datatable') as varchar(10)), " 
                    + (r != null ? r.ID : "0") + ", '" + textBox1.Text 
                    + "'); select id, treekey from datatable "
                    + "where id = ident_current('datatable')", conn);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    Form1.DataRows.MyRow r1 = new Form1.DataRows.MyRow();
                    r1.ID = dr["id"].ToString();
                    r1.ParentID = (r != null ? r.ID : "0");
                    r1.TreeKey = dr["treekey"].ToString();
                    r1.Data = textBox1.Text;
                    TreeNode tn2 = new TreeNode(r1.Data + " [" + r1.TreeKey
                                                  + "]");
                    tn2.Tag = r1;
                    tn.Nodes.Add(tn2);
                    this.Close();
                }
                dr.Close();
                conn.Close();
            }
            else
            {
                MessageBox.Show("Please enter some data.");
            }
        }
    }
}