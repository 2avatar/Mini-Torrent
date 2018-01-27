using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Registeration : System.Web.UI.Page
{

    private static string WARNING_FILL_INFORMATION = "* Please fill information";
    private static string WARNING_NOT_AVAILABLE_USERNAME = "* Your Username is not available, please choose another id";
    private static string WARNING_PASSWORDS_UNCOMFIRMED = "* Your passwords do not match, please try again";
    private static string SUCCESFULL_INPUT_INFORMATION = "Congratulations on your new account";
    private static string REFRESH = "";
    private bool allFieldsFilled = true;
    private bool usernameValidate = true;
    private bool passwordsMatch = true;





    protected void Page_Load(object sender, EventArgs e)
    {
      
    }

    protected void ButtonSubmit_Click(object sender, EventArgs e)
    {
        refreshLabels();
        checkAllFields();

        if (allFieldsFilled && passwordsMatch && usernameValidate)
        {
            var ds = new DataSetUsersTableAdapters.UsersTableAdapter();

            ds.Insert(
                TextBoxUsername.Text,
                ComputeHash(TextBoxPassword.Text),
                false
                );

            refreshTextBox();
      
            LabelSuccessful.Text = SUCCESFULL_INPUT_INFORMATION;
        }
    }

    private void refreshLabels()
    {
        LabelUsername.Text = REFRESH;
        LabelPassword.Text = REFRESH;       
    }

    private void refreshTextBox()
    {
        TextBoxUsername.Text = REFRESH;
        TextBoxPassword.Text = REFRESH;
        TextBoxPasswordConfirm.Text = REFRESH;
    }

    private void checkAllFields()
    {
        checkUsername();
        checkPasswords();
    }

    private bool validateUsernameIsAvailable()
    {
        var ds = new DataSetUsersTableAdapters.UsersTableAdapter();
        return ds.IsUsernameAvailable(TextBoxUsername.Text).Value == 0 ? true : false;
    }

    private void checkUsername()
    {
        if (string.IsNullOrEmpty(TextBoxUsername.Text))
        {
            LabelUsername.Text = WARNING_FILL_INFORMATION;
            setAllFieldsFilledFalse();
        }
        else
        {
            if (!validateUsernameIsAvailable())
            {
                LabelUsername.Text = WARNING_NOT_AVAILABLE_USERNAME;
                usernameValidate = false;
            }
        }
    }

    //private void checkFolderPath()
    //{
    //    if (string.IsNullOrEmpty(TextBoxFolderPath.Text))
    //    {
    //        LabelFolderPath.Text = WARNING_FILL_INFORMATION;
    //        setAllFieldsFilledFalse();
    //    }
    //    else
    //    {
    //        if (!validateFolderPath())
    //        {
    //            LabelFolderPath.Text = WARNING_BAD_FILE_PATH;
    //            correctFolderPath = false;
    //        }
    //    }
    //}

    //private bool validateFolderPath()
    //{
    //    string reg = "^([a-zA-Z]:)?(\\\\[^<>:\"/\\\\|?*]+)+\\\\?$";
    //    Regex folderPathRegularExpression = new Regex(reg);

    //    return folderPathRegularExpression.IsMatch(TextBoxFolderPath.Text);
    //}

    private void checkPasswords()
    {
        if (string.IsNullOrEmpty(TextBoxPassword.Text) || string.IsNullOrEmpty(TextBoxPasswordConfirm.Text))
        {
            LabelPassword.Text = WARNING_FILL_INFORMATION;
            setAllFieldsFilledFalse();
        }
        else
        {
            if (!validatePasswords())
            {
                LabelPassword.Text = WARNING_PASSWORDS_UNCOMFIRMED;
                passwordsMatch = false;
            }
        }
    }

    private bool validatePasswords()
    {
        return TextBoxPassword.Text.Equals(TextBoxPasswordConfirm.Text);
    }

    private void setAllFieldsFilledFalse()
    {
        allFieldsFilled = false;
    }

    public string ComputeHash(string hash)
    {
        SHA256 sha256 = SHA256.Create();
        return byteToString(sha256.ComputeHash(stringToByte(hash)));
    }

    public byte[] stringToByte(string buffer)
    {
        return Encoding.UTF8.GetBytes(buffer);
    }

    public string byteToString(byte[] buffer)
    {
        return BitConverter.ToString(buffer).Replace("-", "");
    }
}






