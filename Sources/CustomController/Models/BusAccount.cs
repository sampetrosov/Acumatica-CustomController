using PX.Data;
using PX.Objects.CR;
using System;

namespace CustomController.Models
{
    public class BusAccount
    {
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string BillingEmail { get; set; }
        public string ShippingAddressLine1 { get; set; }
        public string ShippingAddressLine2 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }
        public string ShippingCountry { get; set; }
        public string BillingAddressLine1 { get; set; }
        public string BillingAddressLine2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZip { get; set; }
        public string BillingCountry { get; set; }

        public static bool CreateBusAccount(BusAccount record)
        {
            if (ValidateData(record))
            {
                try
                {
                    BusinessAccountMaint accountMaint = PXGraph.CreateInstance<BusinessAccountMaint>();
                    BAccount account = new BAccount
                    {
                        AcctCD = record.CompanyName,
                        AcctName = record.CompanyName,
                        Type = "PR",
                    };
                    account = accountMaint.BAccount.Insert(account);
                    Contact defContact = PXCache<Contact>.CreateCopy(PXSelectBase<Contact, PXSelect<Contact, Where<Contact.contactID, Equal<Current<BAccount.defContactID>>>>.Config>.SelectSingleBound(accountMaint, new object[]
                    {
                        account
                    }, Array.Empty<object>()));
                    Guid? defContactNoteID = defContact.NoteID;
                    defContact.ContactType = "AR";
                    defContact.FullName = record.CompanyName;
                    defContact.ContactID = account.DefContactID;
                    defContact.BAccountID = account.BAccountID;
                    defContact.DuplicateStatus = "NV";
                    defContact.DuplicateFound = new bool?(false);
                    defContact.WorkgroupID = null;
                    defContact.OwnerID = null;
                    defContact.ClassID = null;
                    defContact.EMail = record.Email;
                    defContact.FirstName = record.FirstName;
                    defContact.LastName = record.LastName;
                    defContact.Phone1 = record.Phone;
                    defContact.NoteID = defContactNoteID;
                    defContact = accountMaint.DefContact.Update(defContact);
                    Address defAddress = PXSelectBase<Address, PXSelect<Address, Where<Address.addressID, Equal<Required<Contact.defAddressID>>>>.Config>.Select(accountMaint, new object[]
                    {
                        account.DefAddressID
                    });
                    if (defAddress == null)
                    {
                        return false;
                    }
                    defAddress.AddressLine1 = record.BillingAddressLine1;
                    defAddress.AddressLine2 = record.BillingAddressLine2;
                    defAddress.City = record.BillingCity;
                    defAddress.CountryID = record.BillingCountry;
                    defAddress.State = record.BillingState;
                    defAddress.PostalCode = record.BillingZip;
                    accountMaint.AddressCurrent.Cache.Clear();
                    defAddress = accountMaint.AddressCurrent.Update(defAddress);
                    account.DefAddressID = defAddress.AddressID;
                    accountMaint.BAccount.Update(account);
                    accountMaint.Save.Press();
                    return true;
                }
                catch (Exception exc)
                {
                    PXTrace.WriteError(exc);
                }
            }
            return false;
        }

        public static bool ValidateData(BusAccount record)
        {
            return true;
        }
    }
}
