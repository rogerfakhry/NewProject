using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
namespace Spotless.Models
{

    #region Models
    [MetadataType(typeof(CustomerValidation))]
    public partial class Customer
    {
        public string Type { get; set; }
        public string Customer_Services { get; set; }

    }

    public partial class CustomerValidation
    {
        [Required]
        [Display(Name = "fullName")]
        public string fullName { get; set; }

        [Required]
        [Display(Name = "Is Active?")]
        public bool isActive { get; set; }

        [Required]
        [Display(Name = "CustomerType")]
        public string typeId { get; set; }

        ///public IEnumerable<CustomerService> CustomerServices { get; set; }
    }
    #endregion


    #region Repository
    public class CustomerRepository
    {

        private DataClassesDataContext dc = new DataClassesDataContext();
        private string sectionName = "Customer";

        #region Create
        public void Add(Customer entry)
        {
            dc.Customers.InsertOnSubmit(entry);
        }
        #endregion

        #region Read
        public Customer GetBiId(int? id)
        {
            return dc.Customers.FirstOrDefault(d => d.id == id);
        }
        public Customer GetBiIdIsPublished(int? id)
        {
            return dc.Customers.FirstOrDefault(d => d.id == id);
        }

        public IQueryable<Customer> GetAll()
        {
            return dc.Customers;
        }
        public int GetMaxPriority()
        {
            return dc.Customers.Any() ? dc.Customers.Max(d => d.priority) : 0;
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
        public void ManageServices(Customer entry, string[] services)
        {
            dc.CustomerServices.DeleteAllOnSubmit(dc.CustomerServices.Where(d => d.customerId == entry.id));
            dc.SubmitChanges();
            foreach (var entity in services)
            {
                var Item = new CustomerService();
                Item.customerId = entry.id;
                Item.serviceId = Convert.ToInt32(entity);
                Item.dateCreated = DateTime.Now;
                Item.dateModified = DateTime.Now;
                dc.CustomerServices.InsertOnSubmit(Item);
            }
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

        public void Delete(Customer entry)
        {
            dc.Customers.DeleteOnSubmit(entry);
        }
        #endregion

    }
    #endregion

}