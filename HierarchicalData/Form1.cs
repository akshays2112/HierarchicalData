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

        public Form1()
        {
            InitializeComponent();
            TreeNode tn = new TreeNode("Root");
            treeView1.Nodes.Add(tn);
            FillTreeView("", tn);
            SortNodes(null);
        }

        public class DataRows
        {
            public class MyRow
            {
                public string ID;
                public string TreeKey;
                public string ParentID;
                public string Data;
            }

            private int counter = 0;
            private int lastfetched = -1;
            private SqlDataReader sdr;
            private MyRow currentrow = new MyRow();

            public DataRows()
            {
                SqlConnection conn = new SqlConnection(
                            @"Address=192.168.2.110;Database=treeview;UID=sa;PWD=sa;");
                SqlCommand cmd = new SqlCommand(
                            @"select * from datatable order by treekey", conn);
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                sdr = cmd.ExecuteReader();
            }

            public MyRow GetNextRow()
            {
                if (counter > lastfetched)
                {
                    if (sdr.Read())
                    {
                        lastfetched = counter;
                        counter++;
                        currentrow = new MyRow();
                        currentrow.ID = sdr["ID"].ToString();
                        currentrow.ParentID = sdr["ParentID"].ToString();
                        currentrow.TreeKey = sdr["TreeKey"].ToString();
                        currentrow.Data = sdr["Data"].ToString();
                        return currentrow;
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (counter == lastfetched)
                {
                    counter++;
                }

                return currentrow;
            }

            public void MovePrev() { --counter; }
        }

        private void FillTreeView(string currKeyRoot, TreeNode tn)
        {
            DataRows.MyRow row = drows.GetNextRow();
            while (row != null)
            {
                if (NodeStartsWith(row.TreeKey, currKeyRoot))
                {
                    TreeNode tnChild = new TreeNode(row.Data + "["
                                                   + row.TreeKey + "]");
                    tnChild.Tag = row;
                    tn.Nodes.Add(tnChild);
                    FillTreeView(row.TreeKey, tnChild);
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
            if (currKeyRoot == null || currKeyRoot.Length == 0 ||
                treekey.StartsWith(currKeyRoot + "."))
                return true;
            return false;
        }

        private void addNodeToolStripMenuItem_Click(object sender,
                                                    EventArgs e)
        {
            TreeNode tn = treeView1.SelectedNode;
            if (tn != null)
            {
                DataRows.MyRow r = tn.Tag as DataRows.MyRow;
                AddNameForm frm = new AddNameForm();
                if (r != null)
                {
                    frm.r = r;
                }
                frm.tn = tn;
                frm.ShowDialog();
            }
        }

        private void deleteNodeToolStripMenuItem_Click(object sender,
                                                       EventArgs e)
        {
            TreeNode tn = treeView1.SelectedNode;
            if (tn != null)
            {
                DataRows.MyRow r = tn.Tag as DataRows.MyRow;
                if (r != null)
                {
                    SqlConnection conn = new SqlConnection(
                                @"Data Source=192.168.2.110;Initial Catalog=treeview;User Id=sa;Password=sa;");
                    conn.Open();
                    if (MessageBox.Show(
                         "Do you want to delete all child nodes?",
                         "Delete Node", MessageBoxButtons.YesNo)
                                                         == DialogResult.Yes)
                    {
                        SqlCommand cmd = new SqlCommand(
                            "delete datatable where treekey = '" + r.TreeKey
                            + "' or treekey like '" + r.TreeKey + ".%'",
                            conn);
                        cmd.ExecuteNonQuery();
                        tn.Parent.Nodes.Remove(tn);
                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand(
                                      "delete datatable where treekey = '" +
                                      r.TreeKey + "'", conn);
                        cmd.ExecuteNonQuery();
                        tn.Parent.Nodes.Remove(tn);
                    }
                    conn.Close();
                }
            }
        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode tn = e.Data.GetData("System.Windows.Forms.TreeNode")
                                                          as TreeNode;
            TreeViewHitTestInfo hi = treeView1.HitTest(
                            treeView1.PointToClient(new Point(e.X, e.Y)));

            if (hi.Node != null && tn != null)
            {
                DataRows.MyRow rSrc = tn.Tag as DataRows.MyRow;
                DataRows.MyRow rDest = hi.Node.Tag as DataRows.MyRow;
                SqlConnection conn = new SqlConnection(
                                 @"Data Source=192.168.2.110;Initial Catalog=treeview;User Id=sa;Password=sa;");
                conn.Open();
                int dashpos = rSrc.TreeKey.LastIndexOf('.');
                string subSrc = (dashpos > 0 ? rSrc.TreeKey.Substring(0,
                                 dashpos) : "");
                SqlCommand cmd = new SqlCommand(
                      "update datatable set treekey = '" + rDest.TreeKey
                      + "' + right(treekey, len(treekey) - len('"
                      + subSrc + "')) where left(treekey, len('"
                      + rSrc.TreeKey + "')) + '.' = '"
                      + rSrc.TreeKey + "." + "'", conn);
                cmd.ExecuteNonQuery();
                conn.Close();
                drows = new DataRows();
                treeView1.Nodes.Clear();
                TreeNode tnRoot = new TreeNode("Root");
                treeView1.Nodes.Add(tnRoot);
                FillTreeView("", tnRoot);
                SortNodes(null);
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

        private class SymTreeNode : IComparable
        {
            public TreeNode tn;

            public int CompareTo(object obj)
            {
                return tn.Text.CompareTo(((SymTreeNode)obj).tn.Text);
            }
        }

        private void SortNodes(TreeNode tn)
        {
            if (tn == null)
            {
                ArrayList arlTreeNodes = new ArrayList();
                foreach (TreeNode tn1 in treeView1.Nodes)
                {
                    SortNodes(tn1);
                    SymTreeNode sn = new SymTreeNode();
                    sn.tn = tn1;
                    arlTreeNodes.Add(sn);
                }
                arlTreeNodes.Sort();
                treeView1.Nodes.Clear();
                foreach (SymTreeNode sn in arlTreeNodes)
                {
                    treeView1.Nodes.Add(sn.tn);
                }
            }
            else
            {
                ArrayList arlTreeNodes = new ArrayList();
                foreach (TreeNode tn1 in tn.Nodes)
                {
                    SortNodes(tn1);
                    SymTreeNode sn = new SymTreeNode();
                    sn.tn = tn1;
                    arlTreeNodes.Add(sn);
                }
                arlTreeNodes.Sort();
                tn.Nodes.Clear();
                foreach (SymTreeNode sn in arlTreeNodes)
                {
                    tn.Nodes.Add(sn.tn);
                }
            }
        }
    }
}