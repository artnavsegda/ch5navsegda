using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;
using CH5SampleSSP.Contact;

namespace CH5SampleSSP
{
    public class DemoContactListingLogic
    {

        /// <summary>
        /// This class simulates programing of Contact List selection, for all Touchscreens globally. 
        /// Feedback for Contact page from UI are programmed in the events below 
        /// </summary>

        private IContactList _allContacts;
        private List<ContactUser> _allContactUsers;

        public DemoContactListingLogic(IContactList contacts)
        {
            // Send total number of contacts to UI
            contacts.NumberOfContacts((sig, component) => { sig.UShortValue = GetNumberOfContacts(); });
            var availableContacts = GetContacts();

            _allContacts = contacts;
            _allContactUsers = availableContacts;

            ClearContactSelection();


            // Selected first contact from list by default
            contacts.Contact[0].ContactIsSelected((sig, component) => { sig.BoolValue = availableContacts[0].IsSelected; });
            contacts.SelectedCompany((sig, component) => { sig.StringValue = availableContacts[0].Company; });
            contacts.SelectedEmail((sig, component) => { sig.StringValue = availableContacts[0].Email; });
            contacts.SelectedExtension((sig, component) => { sig.StringValue = availableContacts[0].Extension; });
            contacts.SelectedHomePhone((sig, component) => { sig.StringValue = availableContacts[0].HomePhone; });
            contacts.SelectedImage((sig, component) => { sig.StringValue = availableContacts[0].Image; });
            contacts.SelectedMobilePhone((sig, component) => { sig.StringValue = availableContacts[0].MobilePhone; });
            contacts.SelectedName((sig, component) => { sig.StringValue = availableContacts[0].Name; });
            contacts.SelectedNickName((sig, component) => { sig.StringValue = availableContacts[0].NickName; });
            contacts.SelectedTitle((sig, component) => { sig.StringValue = availableContacts[0].Title; });
            contacts.SelectedWorkPhone((sig, component) => { sig.StringValue = availableContacts[0].WorkPhone; });


            // loops all contacts and sends serial values to UI 
            for (int i = 0; i < contacts.Contact.Length; i++)
            {
                contacts.Contact[i].UserObject = availableContacts[i];
                availableContacts[i].UserSpecifiedObject = contacts.Contact[i];

                IContact contactUser = contacts.Contact[i];

                contactUser.FullName((sig, component) => { sig.StringValue = availableContacts[i].Name; });
                contactUser.Image((sig, component) => { sig.StringValue = availableContacts[i].Image; });
                contactUser.Nickname((sig, component) => { sig.StringValue = availableContacts[i].NickName; });
                contactUser.Company((sig, component) => { sig.StringValue = availableContacts[i].Company; });
                contactUser.Title((sig, component) => { sig.StringValue = availableContacts[i].Title; });
                contactUser.Email((sig, component) => { sig.StringValue = availableContacts[i].Email; });
                contactUser.WorkNumber((sig, component) => { sig.StringValue = availableContacts[i].WorkPhone; });
                contactUser.WorkNumberExtension((sig, component) => { sig.StringValue = availableContacts[i].Extension; });
                contactUser.MobileNumber((sig, component) => { sig.StringValue = availableContacts[i].MobilePhone; });
                contactUser.HomeNumber((sig, component) => { sig.StringValue = availableContacts[i].HomePhone; });
                contactUser.ContactIsSelected((sig, component) => { sig.BoolValue = availableContacts[i].IsSelected; });

                // Event called when a contact is clicked on in UI 
                contactUser.SetContactSelected += ContactUser_Selected;
            }
        }

        // Example class for contact user 
        private class ContactUser
        {
            public string Name { get; set; }
            public string NickName { get; set; }
            public string Company { get; set; }
            public string Title { get; set; }
            public string Email { get; set; }
            public string WorkPhone { get; set; }
            public string Extension { get; set; }
            public string MobilePhone { get; set; }
            public string HomePhone { get; set; }
            public string Image { get; set; }
            public bool IsSelected { get; set; }
            public object UserSpecifiedObject { get; set; }
                       
            public ContactUser(){ }
        }

        private List<ContactUser> GetContacts()
        {
            var c = new List<ContactUser>()
            {
                new ContactUser(){ Name="Williams Arden", Image="./assets/img/contacts/user1.jpg", NickName="Bill", Company="ACME Marketing, Inc.", Title="Chief Executive Officer", Email="william.arden@acmemarketing.com", WorkPhone="(212) 555-1568", Extension="15001", MobilePhone="(917) 555-6894", HomePhone="Not Available", IsSelected = true},
                new ContactUser(){ Name="Cheryl Bailey", Image="./assets/img/contacts/user2.jpg", NickName="Cheryl", Company="ACME Marketing, Inc.", Title="Director of Human Resources", Email="cheryl.bailey@acmemarketing.com", WorkPhone="(212) 555-9876", Extension="14787", MobilePhone="(318) 555-8673", HomePhone="(718) 555-1289", IsSelected = false},
                new ContactUser(){ Name="Joseph Chulli", Image="./assets/img/contacts/user3.jpg", NickName="Joe", Company="ACME Marketing, Inc.", Title="Production Video Editor", Email="joseph.chulli@acmemarketing.com", WorkPhone="(212) 555-6893", Extension="16324", MobilePhone="(914) 555-1862", HomePhone="(914) 555-6623", IsSelected = false},
                new ContactUser(){ Name="Gerald Chumba", Image="./assets/img/contacts/user4.jpg", NickName="G", Company="ACME Marketing, Inc.", Title="Production Graphic Artist", Email="gerald.chumba@acmemarketing.com", WorkPhone="(212) 555-8401", Extension="16501", MobilePhone="(917) 555-0058", HomePhone="Not Available", IsSelected = false},
                new ContactUser(){ Name="Aimee Duggan", Image="./assets/img/contacts/user5.jpg", NickName="Aimee", Company="ACME Marketing, Inc.", Title="Director of Production", Email="aimee.duggan@acmemarketing.com", WorkPhone="(212) 555-1083", Extension="15323", MobilePhone="(347) 555-8269", HomePhone="(201) 555-8690", IsSelected = false},
                new ContactUser(){ Name="Maximum Douglas", Image="./assets/img/contacts/user6.jpg", NickName="Max", Company="ACME Marketing, Inc.", Title="Manager, Video Production", Email="maximum.douglas@acmemarketing.com", WorkPhone="(212) 555-6789", Extension="17089", MobilePhone="(917) 555-1178", HomePhone="(718) 555-0186", IsSelected = false},
                new ContactUser(){ Name="Melissa Friedlander", Image="./assets/img/contacts/user7.jpg", NickName="Melissa", Company="ACME Marketing, Inc.", Title="Director of Marketing", Email="melissa.friedlander@acmemarketing.com", WorkPhone="(212) 555-6428", Extension="15021", MobilePhone="(347) 555-0067", HomePhone="Not Available", IsSelected = false},
                new ContactUser(){ Name="Jessica Howard", Image="./assets/img/contacts/user8.jpg", NickName="Jess", Company="ACME Marketing, Inc.", Title="Executive Assitant", Email="jessica.howard@acmemarketing.com", WorkPhone="(212) 555-7123", Extension="15101", MobilePhone="(201) 555-6889", HomePhone="(201) 555-0063", IsSelected = false},
                new ContactUser(){ Name="Aidan Kwong", Image="./assets/img/contacts/user9.jpg", NickName="Aidan", Company="ACME Marketing, Inc. ", Title="Production Audio Engineer", Email="aidan.kwong@acmemarketing.com", WorkPhone="(212) 555-4709", Extension="16383", MobilePhone="(718) 555-9074", HomePhone="Not Available", IsSelected = false},
                new ContactUser(){ Name="Chelsea Latimer", Image="./assets/img/contacts/user10.jpg", NickName="Chelsea", Company="ACME Marketing, Inc. ", Title="Manager of Photography", Email="chealsea.latimer@amemarketing.com", WorkPhone="(212) 555-8203", Extension="15345", MobilePhone="(516) 555-3024", HomePhone="Not Available", IsSelected = false},
                new ContactUser(){ Name="Paolo Lundre", Image="./assets/img/contacts/user11.jpg", NickName="Paul", Company="ACME Marketing, Inc. ", Title="Digital Artist", Email="paolo.lundre@amemarketing.com", WorkPhone="(212) 555-1898", Extension="16881", MobilePhone="(917) 555-8981", HomePhone="Not Available", IsSelected = false},
                new ContactUser(){ Name="Marcus Maris", Image="./assets/img/contacts/user12.jpg", NickName="Marc", Company="ACME Marketing, Inc. ", Title="Manager of Design Services", Email="marcus.maris@amemarketing.com", WorkPhone="(212) 555-6479", Extension="16255", MobilePhone="(516) 555-9764", HomePhone="Not Available", IsSelected = false},
                new ContactUser(){ Name="Stella Marks ", Image="./assets/img/contacts/user13.jpg", NickName="Stella", Company="ACME Marketing, Inc. ", Title="Administrative Assistant", Email="stella.marks@amemarketing.com", WorkPhone="(212) 555-5150", Extension="15865", MobilePhone="(347) 555-7031", HomePhone="Not Available", IsSelected = false},
                new ContactUser(){ Name="Francesca Mazzaros", Image="./assets/img/contacts/user14.jpg", NickName="Fran", Company="ACME Marketing, Inc. ", Title="Chief Financial Officer", Email="francesca.mazzaros@amemarketing.com", WorkPhone="(212) 555-6843", Extension="15011", MobilePhone="(917) 555-4836", HomePhone="(914) 555-1882", IsSelected = false},
                new ContactUser(){ Name="Bradley Meltzer", Image="./assets/img/contacts/user15.jpg", NickName="Brad", Company="ACME Marketing, Inc. ", Title="Director of Sales", Email="bradley.meltzer@amemarketing.com", WorkPhone="(212) 555-3267", Extension="16626", MobilePhone="(718) 555-6237", HomePhone="(718) 555-1970", IsSelected = false},
                new ContactUser(){ Name="Kelly Molloy ", Image="./assets/img/contacts/user16.jpg", NickName="Kelly", Company="ACME Marketing, Inc. ", Title="Corporate Relations", Email="kelly.molloy@amemarketing.com", WorkPhone="(212) 555-1806", Extension="17854", MobilePhone="(212) 555-9004", HomePhone="Not Available", IsSelected = false},
                new ContactUser(){ Name="Ken Nyguen", Image="./assets/img/contacts/user17.jpg", NickName="Ken", Company="ACME Marketing, Inc. ", Title="Sales Associate", Email="ken.nyguen@acmemarketing.com", WorkPhone="(212) 555-4207", Extension="16897", MobilePhone="(516) 555-7232", HomePhone="Not Available", IsSelected = false},
                new ContactUser(){ Name="Jordan North", Image="./assets/img/contacts/user18.jpg", NickName="Jordy", Company="ACME Marketing, Inc. ", Title="Digital Design Artist", Email="jordan.north@acmemarketing.com", WorkPhone="(212) 555-1953", Extension="17119", MobilePhone="(718) 555-3519", HomePhone="Not Available", IsSelected = false},            
                new ContactUser(){ Name="Amara Ogra", Image="./assets/img/contacts/user19.jpg", NickName="Amara", Company="ACME Marketing, Inc. ", Title="Prototype Artist", Email="amara.ogra@acmemarketing.com", WorkPhone="(212) 555-6845", Extension="17184", MobilePhone="(201) 555-4856", HomePhone="Not Available", IsSelected = false},
                new ContactUser(){ Name="Phil Purdue", Image="./assets/img/contacts/user20.jpg", NickName="Phil", Company="ACME Marketing, Inc. ", Title="Chief Technology Officer", Email="phil.purdue@acmemarketing.com", WorkPhone="(212) 555-0087", Extension="15173", MobilePhone="(917) 555-9074", HomePhone="(718) 555-0067", IsSelected = false},
                new ContactUser(){ Name="Rebecca Ramos", Image="./assets/img/contacts/user21.jpg", NickName="Becca", Company="ACME Marketing, Inc. ", Title="Assistant Photography", Email="rebecca.ramos@acmemarketing.com", WorkPhone="(212) 555-9122", Extension="18003", MobilePhone="(201) 555-2219", HomePhone="Not Available", IsSelected = false},
                new ContactUser(){ Name="Bruce Smith", Image="./assets/img/contacts/user22.jpg", NickName="Bruce", Company="ACME Marketing, Inc. ", Title="Director Public Relations", Email="bruce.smith@acmemarketing.com", WorkPhone="(212) 555-6266", Extension="15993", MobilePhone="(646) 555-8154", HomePhone="Not Available", IsSelected = false},
                new ContactUser(){ Name="Lisa Steinhardt", Image="./assets/img/contacts/user23.jpg", NickName="LeeLee", Company="ACME Marketing, Inc.", Title="Director of Social Media", Email="lisa.steinhardt@acmemarketing.com", WorkPhone="(212) 555-3211", Extension="16778", MobilePhone="(914) 555-1223", HomePhone="(914) 555-9983", IsSelected = false},
                new ContactUser(){ Name="Jayden Umphrey", Image="./assets/img/contacts/user24.jpg", NickName="Jay", Company="ACME Marketing, Inc.", Title="Experience Design Artist", Email="jayden.umphrey@acmemarketing.com", WorkPhone="(212) 555-9802", Extension="16092", MobilePhone="(212) 555-2089", HomePhone="Not Available", IsSelected = false},
                new ContactUser(){ Name="Alissa Ziegenbein", Image="./assets/img/contacts/user25.jpg", NickName="Ziggy", Company="ACME Marketing, Inc.", Title="Audio Production Engineer", Email="alissa.ziegenbein@acmemarketing.com", WorkPhone="(212) 555-6742", Extension="16838", MobilePhone="(917) 555-4276", HomePhone="Not Available", IsSelected = false},
                new ContactUser(){ Name="Rebecca Ramos", Image="./assets/img/contacts/user21.jpg", NickName="Becca", Company="ACME Marketing, Inc. ", Title="Assistant Photography", Email="rebecca.ramos@acmemarketing.com", WorkPhone="(212) 555-9122", Extension="18003", MobilePhone="(201) 555-2219", HomePhone="Not Available", IsSelected = false},
                new ContactUser(){ Name="Bruce Smith", Image="./assets/img/contacts/user22.jpg", NickName="Bruce", Company="ACME Marketing, Inc. ", Title="Director Public Relations", Email="bruce.smith@acmemarketing.com", WorkPhone="(212) 555-6266", Extension="15993", MobilePhone="(646) 555-8154", HomePhone="Not Available", IsSelected = false},
                new ContactUser(){ Name="Lisa Steinhardt", Image="./assets/img/contacts/user23.jpg", NickName="LeeLee", Company="ACME Marketing, Inc.", Title="Director of Social Media", Email="lisa.steinhardt@acmemarketing.com", WorkPhone="(212) 555-3211", Extension="16778", MobilePhone="(914) 555-1223", HomePhone="(914) 555-9983", IsSelected = false},
                new ContactUser(){ Name="Jayden Umphrey", Image="./assets/img/contacts/user24.jpg", NickName="Jay", Company="ACME Marketing, Inc.", Title="Experience Design Artist", Email="jayden.umphrey@acmemarketing.com", WorkPhone="(212) 555-9802", Extension="16092", MobilePhone="(212) 555-2089", HomePhone="Not Available", IsSelected = false},
                new ContactUser(){ Name="Alissa Ziegenbein", Image="./assets/img/contacts/user25.jpg", NickName="Ziggy", Company="ACME Marketing, Inc.", Title="Audio Production Engineer", Email="alissa.ziegenbein@acmemarketing.com", WorkPhone="(212) 555-6742", Extension="16838", MobilePhone="(917) 555-4276", HomePhone="Not Available", IsSelected = false},

            };
            return c;
        }

        // Sets number of available contacts to display on UI 
        private ushort GetNumberOfContacts()
        {
            return 25;
        }

        // Clears all selected contacts
        private void ClearContactSelection()
        {
            for (int j = 0; j < _allContacts.Contact.Length; j++)
            {
                _allContacts.Contact[j].ContactIsSelected((sig, component) => { sig.BoolValue = false; });
            }

            for (int i = 0; i < _allContactUsers.Count; i++)
            {
                _allContactUsers[i].IsSelected = false;
            }
        }

        // When contact is clicked on in UI Feedback of True is sent back as well as selected user data
        private void ContactUser_Selected(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
            {
                   
                ClearContactSelection();

                ContactUser user = (ContactUser) ((Contact.Contact) sender).UserObject;
                user.IsSelected = true;

                ((Contact.Contact) sender).ContactIsSelected((sig, component) => { sig.BoolValue = user.IsSelected; });

                _allContacts.SelectedCompany((sig, component) => { sig.StringValue = user.Company; });
                _allContacts.SelectedEmail((sig, component) => { sig.StringValue = user.Email; });
                _allContacts.SelectedExtension((sig, component) => { sig.StringValue = user.Extension; });
                _allContacts.SelectedHomePhone((sig, component) => { sig.StringValue = user.HomePhone; });
                _allContacts.SelectedImage((sig, component) => { sig.StringValue = user.Image; });
                _allContacts.SelectedMobilePhone((sig, component) => { sig.StringValue = user.MobilePhone; });
                _allContacts.SelectedName((sig, component) => { sig.StringValue = user.Name; });
                _allContacts.SelectedNickName((sig, component) => { sig.StringValue = user.NickName; });
                _allContacts.SelectedTitle((sig, component) => { sig.StringValue = user.Title; });
                _allContacts.SelectedWorkPhone((sig, component) => { sig.StringValue = user.WorkPhone; });
            }
        }
    }
}