using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Spotless.Models
{
    #region Models

    [MetadataType(typeof(webpages_RoleValidation))]
    public partial class webpages_Role
    {
        public bool HasPermission(int sectionId, int permissionTypeId)
        {
            return this.Permissions.Any(d => d.sectionId == sectionId && d.permissionTypeId == permissionTypeId);
        }
    }

    public partial class webpages_RoleValidation
    {
        [Required]
        [Display(Name = "Role")]
        public string RoleName { get; set; }
    }
    #endregion

    #region Repository
    public class RoleRepository
    {
        private DataClassesDataContext dc = new DataClassesDataContext();

        #region Read
        public webpages_Role GetBiId(int? id)
        {
            return dc.webpages_Roles.FirstOrDefault(d => d.RoleId == id);
        }

        public IQueryable<webpages_Role> GetAll()
        {
            return dc.webpages_Roles;
        }
        #endregion

        #region Update

        public void ManagePermissions(int roleId, int sectionId, string[] permisionTypeIds)
        {

            foreach (var item in permisionTypeIds)
            {
                Permission entry = new Permission();
                entry.sectionId = sectionId;
                entry.roleId = roleId;
                entry.permissionTypeId = Convert.ToInt32(item);
                entry.dateCreated = DateTime.Now;
                dc.Permissions.InsertOnSubmit(entry);
            }
            Save();
        }

        public void Save()
        {
            dc.SubmitChanges();
        }
        #endregion

        #region Delete
        public void DeleteAllByRole(int id)
        {
            dc.Permissions.DeleteAllOnSubmit(dc.Permissions.Where(d => d.roleId == id));
        }
        #endregion
    }
    #endregion
}