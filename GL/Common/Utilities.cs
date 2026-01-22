using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DAL;
using System.IO;

namespace CombineGym
{
    public static class Utilities
    {
        public static void saveImages(int id, PictureBox pictureBox, string caller)
        {
            DALSetup dal = new DALSetup();
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string exeDir = System.IO.Path.GetDirectoryName(exePath);
            DirectoryInfo binDir = System.IO.Directory.GetParent(exeDir);
            DirectoryInfo appFolderDirInfo = System.IO.Directory.GetParent(binDir.ToString());
            string appFolder = appFolderDirInfo.ToString();
            string fileName=string.Empty;
            string extension = ".png";


            // first remove old picture if exist

            if(caller == "Student")
            {
                Student student = dal.StudentGet(id);
                if (student.Picture != null && student.Picture != string.Empty)
                {
                    fileName = student.Picture;
                    string ExistingFilename = appFolder + "\\Images\\" + fileName;
                    if (File.Exists(ExistingFilename))
                    {
                        File.Delete(ExistingFilename);
                    }
                }

                fileName = "STD_" + DateTime.Now.ToString("yymmssfff") + extension;
                string name = appFolder + "\\Images\\" + fileName;
                pictureBox.Image.Save(name);

                student.Picture = fileName;
                dal.StudentSave(student);

            }
            else if (caller == "Employee")
            {

                Employee employee = dal.EmployeeGet(id);
                if (employee.Picture != null && employee.Picture != string.Empty)
                {
                    fileName = employee.Picture;
                    string ExistingFilename = appFolder + "\\Images\\" + fileName;
                    if (File.Exists(ExistingFilename))
                    {
                        File.Delete(ExistingFilename);
                    }
                }

                fileName = "EMP_" + DateTime.Now.ToString("yymmssfff") + extension;
                string name = appFolder + "\\Images\\" + fileName;
                pictureBox.Image.Save(name);

                employee.Picture = fileName;
                dal.EmployeeSave(employee);

            }

        }

    }
}
