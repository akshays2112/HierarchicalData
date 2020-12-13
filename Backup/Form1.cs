using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Treeview
{
    public partial class Form1 : Form
    {
        public static DataRows drows = new DataRows();
        public static DataTable dt = new DataTable();

        public Form1()
        {
            InitializeComponent();
            TreeNode tn = new TreeNode("Root");
            treeView1.Nodes.Add(tn);
            FillTreeView("", tn);
            DataColumn dc = new DataColumn("ID");
            dt.Columns.Add(dc);
            dc = new DataColumn("TreeKey");
            dt.Columns.Add(dc);
            dc = new DataColumn("ParentID");
            dt.Columns.Add(dc);
            dc = new DataColumn("Data");
            dt.Columns.Add(dc);
        }

        public class DataRows
        {
            private int counter = 0;
            private DataSet ds;

            public DataRows()
            {
                SqlDataAdapter sda = new SqlDataAdapter(@"select * from datatable order by treekey", @"Server=Gandalf\SQL2K;Integrated Security=true;Database=treeview");
                ds = new DataSet();
                sda.Fill(ds);
            }

            public DataRow GetNextRow()
            {
                if (counter >= ds.Tables[0].Rows.Count)
                    return null;
                else
                    return ds.Tables[0].Rows[counter++];
            }

            public void MovePrev() { --counter; }
        }

        private void FillTreeView(string currKeyRoot, TreeNode tn)
        {
            DataRow row = drows.GetNextRow();
            while(row != null)
            {
                if(NodeStartsWith(row["TreeKey"].ToString(), currKeyRoot))
                {
                    TreeNode tnChild = new TreeNode(row["Data"].ToString() + "[" + row["TreeKey"].ToString() + "]");
                    tnChild.Tag = row;
                    tn.Nodes.Add(tnChild);
                    FillTreeView(row["TreeKey"].ToString(), tnChild);
                }
                else
                {
                    drows.MovePrev();
                    return;
                }
                row = drows.GetNextRow();
            }
        }

        private bool NodeStartsWith(string treekey, string currKeyRoot)
        {
            if (currKeyRoot == null || currKeyRoot.Length == 0 || treekey == currKeyRoot || treekey.StartsWith(currKeyRoot + "-"))
                return true;
            return false;
        }

        private void addNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeView1.SelectedNode;
            if (tn != null)
            {
                DataRow r = tn.Tag as DataRow;
                AddNameForm frm = new AddNameForm();
                if (r != null)
                {
                    frm.r = r;
                }
                frm.tn = tn;
                frm.ShowDialog();
            }
        }

        private void deleteNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeView1.SelectedNode;
            if (tn != null)
            {
                DataRow r = tn.Tag as DataRow;
                if (r != null)
                {
                    SqlConnection conn = new SqlConnection(@"Server=Gandalf\SQL2K;Integrated Security=true;Database=treeview");
                    conn.Open();
                    if (MessageBox.Show("Do you want to delete all child nodes?", "Delete Node", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        SqlCommand cmd = new SqlCommand("delete datatable where treekey = '" + r["TreeKey"].ToString() + "' or treekey like '" + r["TreeKey"].ToString() + "-%'", conn);
                        cmd.ExecuteNonQuery();
                        tn.Parent.Nodes.Remove(tn);
                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand("delete datatable where treekey = '" + r["TreeKey"].ToString() + "'", conn);
                        cmd.ExecuteNonQuery();
                        tn.Parent.Nodes.Remove(tn);
                    }
                    conn.Close();
                }
            }
        }

        private void editNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode tn = e.Data.GetData("System.Windows.Forms.TreeNode") as TreeNode;
            TreeViewHitTestInfo hi = treeView1.HitTest(treeView1.PointToClient(new Point(e.X, e.Y)));
            if (hi.Node != null && tn != null)
            {
                DataRow rSrc = tn.Tag as DataRow;
                DataRow rDest = hi.Node.Tag as DataRow;
                SqlConnection conn = new SqlConnection(@"Server=Gandalf\SQL2K;Integrated Security=true;Database=treeview");
                conn.Open();
                int dashpos = rSrc["TreeKey"].ToString().LastIndexOf('-');
                string subSrc = (dashpos > 0 ? rSrc["TreeKey"].ToString().Substring(0, dashpos) : "");
                SqlCommand cmd = new SqlCommand("update datatable set treekey = '" + rDest["TreeKey"].ToString() + "' + right(treekey, len(treekey) - len('" + subSrc + "')) where left(treekey, len('" + rSrc["TreeKey"].ToString() + "')) + '-' = '" + rSrc["TreeKey"].ToString() + "-" + "'", conn);
                cmd.ExecuteNonQuery();
                conn.Close();
                drows = new DataRows();
                treeView1.Nodes.Clear();
                TreeNode tnRoot = new TreeNode("Root");
                treeView1.Nodes.Add(tnRoot);
                FillTreeView("", tnRoot);
            }
            else
            {
                MessageBox.Show("Not found");
            }
        }

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
    }
}