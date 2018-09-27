using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Spotless.Models
{
    #region Models

    [MetadataType(typeof(ServiceValidation))]
    public partial class Service
    {
        public string UrlTitle
        {
            get
            {
                return (this.title + "").Replace(",", "-").Replace("'", "").Replace("!", "").Replace("&", "and").Replace("@", "").Replace(" ", "-").Replace("--", "-");
            }
        }
    }

    public partial class ServiceValidation
    {
        [Required]
        public string title { get; set; }

        // [Required]
        public string description { get; set; }

        public string comPort { get; set; }

      

    }
    #endregion

    #region Repository
    public class ServiceRepository
    {
        private DataClassesDataContext dc = new DataClassesDataContext();
        private string sectionName = "Service";

        #region Create
        public void Add(Service entry)
        {
            dc.Services.InsertOnSubmit(entry);
        }
        #endregion

        #region Read
        public Service GetBiId(int? id)
        {
            return dc.Services.FirstOrDefault(d => d.id == id);
        }
        public IQueryable<Service> GetAll()
        {
            if (IsSortable())
            {
                return dc.Services.OrderBy(d => d.priority);
            }
            else
            {
                return dc.Services;
            }
        }

        public int GetMaxPriority()
        {
            return dc.Services.Any() ? dc.Services.Max(d => d.priority) : 0;
        }

        public bool IsSortable()
        {
            return dc.Sections.Any(d => d.computername.ToLower() == sectionName.ToLower() && d.isSortable);
        }
        #endregion

        #region Update

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


        public void Save()
        {
            dc.SubmitChanges();
        }
        #endregion

        #region Delete
        public void Delete(Service entry)
        {
            dc.Services.DeleteOnSubmit(entry);
        }
        #endregion

    }
    #endregion
}