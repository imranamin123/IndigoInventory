using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity.Migrations;
using GL.EF;

namespace DAL
{
    public class DALSecurity
    {
        public GLEntities db = new GLEntities();
        public SecUser ValidateUser(string username, string password)
        {
            SecUser user = null;
            try
            {
           
                user = db.SecUsers.Where(u => u.username == username && u.password == password && u.StatusTypeID == 1).FirstOrDefault();
                return user;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public int GetUserID(string username)
        {
            SecUser user = null;
            try
            {

                user = db.SecUsers.Where(u => u.username == username && u.StatusTypeID == 1).FirstOrDefault();
                return user.UsersID;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public int ChangePassword(int userID, string oldPassword, string newPassword)
        {
            try
            {
                int HttpStatusCode; // Ok = 200
                var user = db.SecUsers.Where(u => u.UsersID == userID).FirstOrDefault();
                if (user != null)
                {
                    if (user.password != oldPassword)
                    {
                        HttpStatusCode = 401; //   Unauthorized = 401
                    }
                    else
                    {
                        user.password = newPassword;
                        db.SecUsers.AddOrUpdate();
                        db.SaveChanges();
                        HttpStatusCode = 200;
                    }
                }
                else
                {
                    HttpStatusCode = 400; //BadRequest = 400
                }
                return HttpStatusCode;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public  spLoginUser_Result GetLoginUser(int CompanyID, int UserID )
        {
            try
            {
                var loginUser = db.spLoginUser(CompanyID, UserID).FirstOrDefault();
                return loginUser;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SecModule> GetMenuModules(int CompanyID, int UserID)
        {
            List<SecModule> modules = null;
            //List<int> moduleIds = null;

            List<mod> moduleIds = (from r in db.SecPagesRights
                                   where r.IsViewAllowed == true && r.CompanyID == CompanyID && r.UserID == UserID && r.PageNo == null
                                   select new mod { ModuleNo = r.ModuleNo.Value, ModuleOrderNo = r.ModuleOrderNo.Value }).Distinct().OrderBy(r => r.ModuleOrderNo).ToList();

            if (moduleIds.Count() > 0)
            {
                modules = new List<SecModule>();
                foreach (var mod in moduleIds)
                {
                    SecModule module = db.SecModules.Where(x => x.ModuleNo == mod.ModuleNo).FirstOrDefault();
                    modules.Add(module);
                }
            }

            return modules;
        }

        public List<spMenuPagesRightsList_Result> GetMenuPageRightsList(int CompanyID, int? UserID)
        {
            var MenuPageRightsList = db.spMenuPagesRightsList(CompanyID, UserID).ToList();
            return MenuPageRightsList;
        }

        //public List<spMenuPagesRightsList_Result> GetMenuPageRightsList2(short? CompanyID, int? UserID)
        //{
        //    int mi = 0;
        //    var menu = (from m in db.SecModules
        //                join p in db.SecPages
        //                on m.ModuleNo equals p.ModuleNo
        //                select new
        //                {
        //                    ModuleNo = m.ModuleNo,
        //                    PageNo = p.PageNo
        //                }).ToList();

        //    List<spMenuPagesRightsList_Result> CrossJoinResult = (from user in db.SecUsers
        //                           from modpages in menu
        //                           select new
        //                           {
        //                               UserID = user.UsersID,
        //                               username = user.username,
        //                               ModuleNo = modpages.ModuleNo,
        //                               PageNo = modpages.PageNo
        //                           }).ToList();

        //    //foreach (var item in CrossJoinResult)
        //    //{
        //    //    Console.WriteLine($"Name : {item.Name}, Subject: {item.SubjectName}");
        //    //}
        //    var MenuPageRightsList = db.spMenuPagesRightsList(CompanyID, UserID).ToList();
        //    return MenuPageRightsList;
        //}

        public List<spUserPagesRights_Result> GetUserPagesRights(short CompanyID, int? UserID)
        {

            var UserPagesRights = GetUserPagesRightsWithMenu(db.spUserPagesRights(CompanyID, UserID).ToList(),CompanyID, UserID.Value);
            return UserPagesRights;
        }

        private List<spUserPagesRights_Result> GetUserPagesRightsWithMenu(List<spUserPagesRights_Result> UserPagesRights, int CompanyID, int UserID)
        {
            List<spUserPagesRights_Result> listReturn = new List<spUserPagesRights_Result>();
            List<SecModule> modules = db.SecModules.ToList();
            foreach(var mod in modules)
            {
                spUserPagesRights_Result spUserPagesRight = null;
                List<spUserPagesRights_Result> UserPageRights=new List<spUserPagesRights_Result>();

                var modRights = UserPagesRights.Where(x => x.CompanyID == CompanyID && x.UsersID == UserID && x.ModuleNo == mod.ModuleNo).ToList();
                UserPageRights.AddRange(modRights);

                var SecPagesRightsExists = db.SecPagesRights.Where(x => x.CompanyID == CompanyID && x.UserID == UserID && x.ModuleNo == mod.ModuleNo && x.PageNo == null).FirstOrDefault(); ; //UserPagesRights.Where(x => x.CompanyID == CompanyID && x.UsersID == UserID && x.ModuleNo == mod.ModuleNo && x.PageNo == null).FirstOrDefault();

                if (SecPagesRightsExists == null)
                {
                    spUserPagesRight = new spUserPagesRights_Result() { CompanyID = CompanyID, UsersID = UserID, ModuleNo = mod.ModuleNo, ModuleName = mod.ModuleName };
                    UserPageRights.Insert(0, spUserPagesRight);
                }
                else
                {
                    var right = new spUserPagesRights_Result
                    {
                        PageRightsID = SecPagesRightsExists.PageRightsID,
                        CompanyID = Convert.ToInt16(SecPagesRightsExists.CompanyID.Value),
                        UsersID = SecPagesRightsExists.UserID.Value,
                        username = modRights[0].username,
                        ModuleNo = SecPagesRightsExists.ModuleNo,
                        ModuleName = modRights[0].ModuleName,
                        IsViewAllowed = SecPagesRightsExists.IsViewAllowed
                    };

                    UserPageRights.Insert(0, right);
                }

                listReturn.AddRange(UserPageRights);
            }

            return listReturn;
        }

        //public List<spUserPagesRights_Result> GetUserPagesRights(short? CompanyID, int? UserID)
        //{
        //    var UserPagesRights = db.spUserPagesRights(CompanyID, UserID).ToList();
        //    return UserPagesRights;
        //}
        public void UserPagesRightsSave(List<SecPagesRight> SecPagesRights)
        {
            try
            {
                foreach (var right in SecPagesRights)
                {
                    if (right.PageRightsID > 0 || (right.PageRightsID == 0 && right.IsViewAllowed == true))
                    {
                        db.SecPagesRights.AddOrUpdate(right);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<spGetMenuPages_Result> GetMenuPages(int CompanyID, int UserID)
        {
            var GetMenuPages = db.spGetMenuPages(CompanyID, UserID).ToList(); //db.spGetMenuPageRights(CompanyID, UserID).ToList();
            return GetMenuPages;
        }

        public void UpdateModuleOrder(int CompanyID, int UserID, List<int> modIdList)
        {
            int count = 1;
            foreach (var modId in modIdList)
            {
                try
                {
                    List<SecPagesRight> PagesRights = db.SecPagesRights.Where(x => x.CompanyID == CompanyID && x.UserID == UserID && x.ModuleNo == modId && x.IsViewAllowed == true).ToList();
                    foreach (var userRight in PagesRights)
                    {
                        userRight.ModuleOrderNo = count;
                        db.SecPagesRights.AddOrUpdate(userRight);
                        db.SaveChanges();
                    }
                }
                catch (Exception)
                {
                    continue;
                }
                count++;
            }
        }
        public void UpdateModulePageOrder(int CompanyID, int UserID, int ModuleNo, List<int> pageIdList)
        {
            int count = 1;
            foreach (var pageId in pageIdList)
            {
                try
                {
                    SecPagesRight PagesRight = db.SecPagesRights.Where(x => x.CompanyID == CompanyID && x.UserID == UserID && x.ModuleNo == ModuleNo && x.PageNo == pageId).FirstOrDefault();
                    PagesRight.PageOrderNo = count;
                    db.SecPagesRights.AddOrUpdate(PagesRight);
                    db.SaveChanges();

                }
                catch (Exception)
                {
                    continue;
                }
                count++;
            }
        }


        public class mod
        {
            public int ModuleNo { get; set; }
            public int ModuleOrderNo { get; set; }
        }
    }
}

