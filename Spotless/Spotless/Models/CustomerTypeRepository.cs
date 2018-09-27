using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
namespace Spotless.Models
{

    #region Models
    [MetadataType(typeof(CustomerTypeValidation))]
    public partial class CustomerType
    {
    }

    public partial class CustomerTypeValidation
    {
        [Required]
        [Display(Name = "name")]
        public string name  { get; set; }
    }
    #endregion


    #region Repository
    public class CustomerTypeRepository
    {

        private DataClassesDataContext dc = new DataClassesDataContext();
        private string sectionName = "CustomerType";

        #region Create
        public void Add(CustomerType entry)
        {
            dc.CustomerTypes.InsertOnSubmit(entry);
        }
        #endregion

        #region Read
        public CustomerType GetBiId(int? id)
        {
            return dc.CustomerTypes.FirstOrDefault(d => d.id == id);
        }
        public CustomerType GetBiIdIsPublished(int? id)
        {
            return dc.CustomerTypes.FirstOrDefault(d => d.id == id);
        }

        public IQueryable<CustomerType> GetAll()
        {
            return dc.CustomerTypes;
        }
        public int GetMaxPriority()
        {
            return dc.CustomerTypes.Any() ? dc.CustomerTypes.Max(d => d.priority) : 0;
        }

        public bool IsSortable()
        {
            return dc.Sections.Any(d => d.computername.ToLower() == sectionName.ToLower() && d.isSortable);
        }
        #endregion

        #region Update
        public void Save()
        {
            dc.SubmitChanges();
        }
        public string SortGrid(int newIndex, int oldIndex, int id)
        {
            var entry = GetBiId(id);
            var allData = GetAll();
            var steps = newIndex - oldIndex;
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
        #endregion

        #region Delete
        public void DeleteAllBySection(int id)
        {
            dc.Permissions.DeleteAllOnSubmit(dc.Permissions.Where(d => d.sectionId == id));
        }

        public void Delete(CustomerType entry)
        {
            dc.CustomerTypes.DeleteOnSubmit(entry);
        }
        #endregion

    }
    #endregion

}