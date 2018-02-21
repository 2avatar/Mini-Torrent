using System;
using System.Data;

public partial class Information : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindUserGrid();
            BindFilesGrid();
        }
    }

    private DataTable GetFilesRecords()
    {

        DataTable dt = new DataTable();
        dt.Columns.AddRange(new DataColumn[3] { new DataColumn("File Name", typeof(string)),
                            new DataColumn("File Size", typeof(int)),
                            new DataColumn("User Name", typeof(string))
                           });

        var dsUsers = new DataSetUsersTableAdapters.UsersTableAdapter();

        foreach (var u in dsUsers.GetAllActiveUsers())
        {
            var dsFiles = new DataSetUsersTableAdapters.FilesTableAdapter();

            foreach (var f in dsFiles.GetFilesByUsername(u.Username))
            {
                dt.Rows.Add(f.Name as string, f.Size as int?, u.Username as string);
            }

            //DirectoryInfo d = new DirectoryInfo(@u.File_Path);
            //try
            //{
            //    FileInfo[] Files = d.GetFiles();
            //    foreach (var f in Files)
            //    {
            //        dt.Rows.Add(f.Name as string, u.Username as string);
            //    }
            //}
            //catch { }

        }

        return dt;
    }

    private void BindFilesGrid()
    {
        DataTable dt = GetFilesRecords();
        if (dt.Rows.Count > 0)
        {
            FilesGridView.DataSource = dt;
            FilesGridView.DataBind();
        }
    }

    private void SearchText(string strSearchText)
    {
        DataTable dt = GetFilesRecords();
        DataView dv = new DataView(dt);

        if (!String.IsNullOrEmpty(strSearchText))
        {       
            string SearchExpression = null;
            SearchExpression = string.Format("{0} '{1}%'",
            FilesGridView.SortExpression, strSearchText);
            dv.RowFilter = "[File Name] like" + SearchExpression;        
        }

        FilesGridView.DataSource = dv;
        FilesGridView.DataBind();
    }

    private DataTable GetUserRecords()
    {
        DataTable dt = new DataTable();
        dt.Columns.AddRange(new DataColumn[3] { new DataColumn("Total Active Users", typeof(int)),
                            new DataColumn("Total Users", typeof(int)),
                            new DataColumn("Total Active Files",typeof(int)) });

        var dsUsers = new DataSetUsersTableAdapters.UsersTableAdapter();

        var totalActiveUsers = dsUsers.GetNumberOfActiveUsers() as int?;
        var totalUsers = dsUsers.GetNumberOfTotalUsers() as int?;
        int? totalActiveFiles = 0;

        foreach (var u in dsUsers.GetAllActiveUsers())
        {
            var dsFiles = new DataSetUsersTableAdapters.FilesTableAdapter();
            totalActiveFiles += dsFiles.GetNumberOfFilesByUsername(u.Username);
        
            //DirectoryInfo d = new DirectoryInfo(@u.File_Path);
            //try
            //{
            //    FileInfo[] Files = d.GetFiles();
            //    totalActiveFiles += Files.Length;
            //}
            //catch
            //{

            //}
           
           
        }

        dt.Rows.Add(totalActiveUsers, totalUsers, totalActiveFiles);

        return dt;

    }

    private void BindUserGrid()
    {
        DataTable dt = GetUserRecords();
        if (dt.Rows.Count > 0)
        {
            UserGridView.DataSource = dt;
            UserGridView.DataBind();
        }
    }

    protected void SearchFiles_Click(object sender, EventArgs e)
    {
        SearchText(TextBox1.Text);
    }
}