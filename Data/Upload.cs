using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Data {
    public class Upload {
        public static Boolean removeImageFromRecord(string table, string imageField, string idField, int idValue) {
            if (table.Length > 0 && imageField.Length > 0 && idField.Length > 0 && idValue > 0) {
                try {
                    DB d = new DB(ConfigurationManager.ConnectionStrings["connection"].ConnectionString);
                    d.Runo("update " + table + " set " + imageField + " = '' where " + idField + " = " + idValue);
                    return true;
                } catch (Exception xxx) { return false; }
            }
            return false;
        }
        public static string saveFile(FileUpload file) {
            return saveFile(file, "");
        }
        public static string saveFile(FileUpload file, string subFolder) { // return filename if uploaded
            if ((file.PostedFile != null) && (file.PostedFile.ContentLength > 0)) {
                string fn = System.IO.Path.GetFileName(file.PostedFile.FileName);
                string originalFilename = fn;
                string saveloc = "";
                Random rnd = new Random();
                fn = rnd.Next() + fn;
                originalFilename = fn; 
                if (subFolder.Length > 0)
                    fn = subFolder + "/" + fn;
                if (ConfigurationManager.AppSettings["UploadSetPath"].IndexOf(":\\") >= 0)
                    saveloc = ConfigurationManager.AppSettings["UploadSetPath"] + "\\" + fn;
                else
                    saveloc = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["UploadSetPath"]) + "\\" + fn;

                try {
                    file.PostedFile.SaveAs(saveloc);
                    // success
                    return originalFilename;
                } catch (Exception xxx) {
                    throw new Exception("Error saving file to : " + saveloc +"; " + xxx.Message.ToString());
                }
            }

            return "";
        }
    }
}