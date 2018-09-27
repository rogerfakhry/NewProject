using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Spotless.Models
{
    #region Models

    [MetadataType(typeof(ContactValidation))]
    public partial class Contact
    {

    }

    public partial class ContactValidation
    {
        [Required]
        [Display(Name = "Full Name")]
        public string fullName { get; set; }

        [Required]
        public string email { get; set; }

        public string mobile { get; set; }

        [Required]
        public string comment { get; set; }
    }
    #endregion

    #region Repository
    public class ContactRepository
    {
        private DataClassesDataContext dc = new DataClassesDataContext();
        private string sectionName = "Contact";

        #region Create
        public void Add(Contact entry)
        {
            dc.Contacts.InsertOnSubmit(entry);
        }
        #endregion

        #region Read
        public Contact GetBiId(int? id)
        {
            return dc.Contacts.FirstOrDefault(d => d.id == id);
        }

        public IQueryable<Contact> GetAll()
        {
            if (IsSortable())
            {
                return dc.Contacts;
            }
            else
            {
                return dc.Contacts.OrderByDescending(d => d.dateCreated);
            }
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
        #endregion

        #region Delete
        public void Delete(Contact entry)
        {
            dc.Contacts.DeleteOnSubmit(entry);
        }
        #endregion

    }
    #endregion
}