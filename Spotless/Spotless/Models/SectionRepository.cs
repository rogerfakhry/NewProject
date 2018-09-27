using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Spotless.Models
{
    #region Models
    [MetadataType(typeof(SectionValidation))]
    public partial class Section
    {
        public string relatedPermissions { get; set; }

        public bool HasPermission(int roleId, int permissionTypeId)
        {
            return this.Permissions.Any(d => d.roleId == roleId && d.permissionTypeId == permissionTypeId);
        }
    }

    public partial class SectionValidation
    {
        [Required]
        public string title { get; set; }

        [Required]
        [Display(Name = "Computername")]
        public string computername { get; set; }

        [Display(Name = "Publishable")]
        public string isPublishable { get; set; }

        [Display(Name = "Show On Menu")]
        public string showOnMenu { get; set; }

        [Display(Name = "Sortable")]
        public string isSortable { get; set; }

        [Display(Name = "Permissions")]
        public string relatedPermissions { get; set; }

    }

    public class EPermission
    {
        public Section sectionEntry { get; set; }
        public bool CanRead { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanPublish { get; set; }
        public bool CanSort { get; set; }
    }
    #endregion

    #region Repository
    public class SectionRepository
    {
        private DataClassesDataContext dc = new DataClassesDataContext();
        private string sectionName = "Section";

        #region Create
        public void Add(Section entry)
        {
            dc.Sections.InsertOnSubmit(entry);
        }
        #endregion

        #region Read
        public Section GetBiId(int? id)
        {
            return dc.Sections.FirstOrDefault(d => d.id == id);
        }
        public Section GetBiSectionName(string sectionName)
        {
            return dc.Sections.FirstOrDefault(d => d.computername == sectionName);
        }


        public IQueryable<Section> GetAll()
        {
            if (IsSortable())
            {
                return dc.Sections.OrderBy(d => d.priority);
            }
            else
            {
                return dc.Sections;
            }
        }


        public IQueryable<Section> GetAllOnMenu()
        {
            if (IsSortable())
            {
                return dc.Sections.Where(d => d.showOnMenu).OrderBy(d => d.priority);
            }
            else
            {
                return dc.Sections.Where(d => d.showOnMenu);
            }
        }


        public int GetMaxPriority()
        {
            return dc.Sections.Max(d => d.priority);
        }

        public bool IsSortable()
        {
            return dc.Sections.Any(d => d.computername.ToLower() == sectionName.ToLower() && d.isSortable);
        }

        public bool GetPermission(string sectionName, int userId, string permissionType)
        {
            return dc.Permissions.Any(d => d.PermissionType.computername == permissionType && d.Section.computername.ToLower() == sectionName.ToLower() && d.webpages_Role.webpages_UsersInRoles.Any(e => e.UserId == userId));
        }

        public EPermission GetPermissions(string sectionName, int userId)
        {
            EPermission entry = new EPermission();
            entry.sectionEntry = GetBiSectionName(sectionName);
            entry.CanRead = GetPermission(sectionName, userId, "can-read");
            entry.CanCreate = GetPermission(sectionName, userId, "can-add");
            entry.CanUpdate = GetPermission(sectionName, userId, "can-edit");
            entry.CanDelete = GetPermission(sectionName, userId, "can-delete");
            entry.CanPublish = GetPermission(sectionName, userId, "can-publish");
            entry.CanSort = GetPermission(sectionName, userId, "can-sort");
            return entry;
        }

        public IQueryable<Section> GetMenuSections(int userId)
        {
            return dc.Permissions.Where(d => d.PermissionType.computername == "can-read" && d.Section.showOnMenu && d.webpages_Role.webpages_UsersInRoles.Any(e => e.UserId == userId)).Select(d => d.Section).OrderBy(d => d.priority);
        }

        #endregion

        #region Update

        public void ManagePermissions(int id, int roleId, string[] permisionTypeIds)
        {

            foreach (var item in permisionTypeIds)
            {
                Permission entry = new Permission();
                entry.sectionId = id;
                entry.roleId = roleId;
                entry.permissionTypeId = Convert.ToInt32(item);
                entry.dateCreated = DateTime.Now;
                dc.Permissions.InsertOnSubmit(entry);
            }
            Save();
        }

        public string SortGrid(int newIndex, int oldIndex, int id)
        {
            var allData = GetAll();
            var steps = newIndex - oldIndex;
            var entry = GetBiId(id);
            if (steps > 0)
            {
                int lastRow = 0;
                int counter = 0;
                var tempAllData = allData.Where(d => d.priority > entry.priority).OrderBy(d => d.priority).Take(steps).ToList();
                foreach (var item in tempAllData)
                {
                    counter++;
                    if (counter == tempAllData.Count)
                    {
                        lastRow = item.priority;
                    }
                    item.priority -= 1;
                    Save();
                }

                entry.priority = lastRow;
                Save();
            }
            else
            {
                int firstRow = 0;
                int counter = 0;
                int recordsToSkip = allData.Where(d => d.priority < entry.priority).Count();
                var tempAllData = allData.Where(d => d.priority < entry.priority).OrderBy(d => d.priority).Skip(((recordsToSkip + steps) < 0 ? 0 : (recordsToSkip + steps))).Take(recordsToSkip).ToList();
                foreach (var item in tempAllData)
                {
                    counter++;
                    if (counter == 1)
                    {
                        firstRow = item.priority;
                    }
                    item.priority += 1;
                    Save();
                }
                entry.priority = firstRow;
                Save();
            }
            return "success";
        }

        public void Save()
        {
            dc.SubmitChanges();
        }
        #endregion

        #region Delete

        public void DeleteAllBySection(int id)
        {
            dc.Permissions.DeleteAllOnSubmit(dc.Permissions.Where(d => d.sectionId == id));
        }

        public void Delete(Section entry)
        {
            dc.Sections.DeleteOnSubmit(entry);
        }
        #endregion

    }
    #endregion
}