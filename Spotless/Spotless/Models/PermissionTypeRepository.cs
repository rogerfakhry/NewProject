using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Spotless.Models
{
    #region Models

    [MetadataType(typeof(PermissionTypeValidation))]
    public partial class PermissionType
    {

    }

    public partial class PermissionTypeValidation
    {
        [Required]
        public string title { get; set; }

        [Required]
        [Display(Name = "Computername")]
        public string computername { get; set; }

    }
    #endregion

    #region Repository
    public class PermissionTypeRepository
    {
        private DataClassesDataContext dc = new DataClassesDataContext();

        #region Create
        public void Add(PermissionType entry)
        {
            dc.PermissionTypes.InsertOnSubmit(entry);
        }
        #endregion

        #region Read
        public PermissionType GetBiId(int? id)
        {
            return dc.PermissionTypes.FirstOrDefault(d => d.id == id);
        }

        public IQueryable<PermissionType> GetAll()
        {
            return dc.PermissionTypes;
        }

        //public IQueryable<JsonPermissionType> GetAllAsJson()
        //{
        //    return GetAll();
        //}

        #endregion

        #region Update
        public void Save()
        {
            dc.SubmitChanges();
        }
        #endregion

        #region Delete
        public void Delete(PermissionType entry)
        {
            dc.PermissionTypes.DeleteOnSubmit(entry);
        }
        #endregion

    }
    #endregion
}