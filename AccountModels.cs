using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
//using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAP.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }
    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
    public class UserModel
    {
        public string bankid { get; set; }
        ///<summary>
        /// Gets or sets Name.
        ///</summary>
        public string bankname { get; set; }
        ///<summary>
        /// Gets or sets Country.
        ///</summary>
        public string shortname { get; set; }

        public string rating { get; set; }
        public string rating_id { get; set; }

        public string username { get; set; }

        public List<UserModel> ulist { get; set; }

        public List<UserModel> r_list { get; set; }

        //public int ID { get; set; }
        //public string Name { get; set; }
        //public string SurName { get; set; }

        //public static List<UserModel> getUsers()
        //{

        //    //DataTable table = new DataTable();
        //    //string constr = ConfigurationManager.ConnectionStrings["DAP"].ConnectionString;
        //    //using (SqlConnection con = new SqlConnection(constr))
        //    //{
        //    //    string query = "select bankid,bankname,shortname,rating from Banks ";
        //    //    using (SqlCommand cmd = new SqlCommand(query, con))
        //    //    {
        //    //        SqlDataAdapter ds = new SqlDataAdapter(cmd);
        //    //        ds.Fill(table);
        //    //    }
        //    //}

        //    //List<UserModel> users = new List<UserModel>();

        //    //for (int i = 0; i < table.Rows.Count; i++)
        //    //{
        //    //    //  bankname="Anubhav",shortname="Chaudhary",rating="AA+"

        //    //    users.Add(new UserModel()
        //    //    {
        //    //        bankid = table.Rows[i]["bankid"].ToString(),
        //    //        bankname = table.Rows[i]["bankname"].ToString(),
        //    //        shortname = table.Rows[i]["shortname"].ToString(),
        //    //        rating = table.Rows[i]["rating"].ToString()
        //    //    });
        //    //}

        //    //int f = users.Count;

        //    ////{  
        //    ////    new UserModel (){ bankid="1", bankname="Anubhav",shortname="Chaudhary",rating="AA+" },   
        //    ////    new UserModel (){ bankid="2", bankname="Mohit",shortname="Singh",rating="AA+" },  
        //    ////    new UserModel (){ bankid="3", bankname="Sonu",shortname="Garg",rating="AA+" },  
        //    ////    new UserModel (){ bankid="4", bankname="Shalini",shortname="Goel",rating="AA+" },  
        //    ////    new UserModel (){ bankid="5", bankname="James",shortname="Bond",rating="AA+" },  
        //    ////};
        //    //return users;
        //}


    }
    public class UserLogin
    {
        public string userid { get; set; }
        [Required(ErrorMessage = "UserName is Required")]
        public string username { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        public string password { get; set; }
        public string con_password { get; set; }
        public string depertment { get; set; }
        public string depertment_id { get; set; }
        // public string designation { get; set; }
        public List<UserLogin> dept_list { get; set; }
        public string tcount { get; set; }

    }
    
    public class Rating
    {
        public string rating_id { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string rating_name { get; set; }
        [Required(ErrorMessage = "Short Name is Required")]
        public string rating_shortname { get; set; }
        [Required(ErrorMessage = "Status is Required")]
        public string rating_status { get; set; }
        public string rating_isdefault { get; set; }
        public bool rating_isdefault_bool { get; set; }
        public List<Rating> rating_list { get; set; }
        public SelectList st_list { get; set; }
    }
    public class Rating_pdf
    {

        ///////////////////////////////////////// for pdf ////////////////////////////

        public string Serial { get; set; }
        public string Name { get; set; }
        public string Shortname { get; set; }
        public string Status { get; set; }
        public string IsDefault { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public List<Rating_pdf> rating_list { get; set; }
        ///////////////////////////////////////// for pdf ////////////////////////////
    }


    public class Bank_Type
    {
        public string BNKTYPE_ID { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string BNKTYPE_NAME { get; set; }
        [Required(ErrorMessage = "Short Name is Required")]
        public string BNKTYPE_SHORTNAME { get; set; }
        [Required(ErrorMessage = "Status is Required")]
        public string BNKTYPE_STATUS { get; set; }
        public string BNKTYPE_ISDEFAULT { get; set; }
        public bool BNKTYPE_ISDEFAULT_BOOL { get; set; }
        public List<Bank_Type> bank_type_list { get; set; }
        //public string BNKTYPE_CREATEDBY { get; set; }          
    }

    public class Bank_Type_pdf
    {

        ///////////////////////////////////////// for pdf ////////////////////////////

        public string Serial { get; set; }
        public string Name { get; set; }
        public string Shortname { get; set; }
        public string Status { get; set; }
        public string IsDefault { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public List<Bank_Type_pdf> Bank_Type_list { get; set; }
        ///////////////////////////////////////// for pdf ////////////////////////////
    
    }


    public class Account_Type
    {
        public string ACCTYPE_ID { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string ACCTYPE_NAME { get; set; }
        [Required(ErrorMessage = "Short Name is Required")]
        public string ACCTYPE_SHORTNAME { get; set; }
        [Required(ErrorMessage = "Status is Required")]
        public string ACCTYPE_STATUS { get; set; }
        public string ACCTYPE_ISDEFAULT { get; set; }
        public bool ACCTYPE_ISDEFAULT_BOOL { get; set; }
        public List<Account_Type> account_type_list { get; set; }

    }
    public class Account_Type_pdf
    {
        ///////////////////////////////////////// for pdf ////////////////////////////
        public string Serial { get; set; }
        public string Name { get; set; }
        public string Shortname { get; set; }
        public string Status { get; set; }
        public string IsDefault { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public List<Account_Type_pdf> account_type_list { get; set; }
        ///////////////////////////////////////// for pdf ////////////////////////////    
    }

    
    public class Bank_Setup
    {
        public string BANK_ID { get; set; }
        [Required(ErrorMessage = "Name is Required")]

        [RegularExpression(@"^[A-Za-z ]+$",ErrorMessage="Kindly Enter Correct Bank Name")]

       // [RegularExpression(@"/^[a-zA-Z][a-zA-Z\\s]+$/",ErrorMessage="Kindly Enter Correct Bank Name")]
     //       ^[a-zA-Z][a-zA-Z\\s]+$

        public string Bank_Name { get; set; }
        [Required(ErrorMessage = "Short Name is Required")]
        [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "Kindly Enter Correct Short Name")]
        public string Bank_ShortName { get; set; }
        //[Required(ErrorMessage = "Code is Required")]
        //[StringLength(4, ErrorMessage = "The Code must contains 4 characters", MinimumLength = 3)]
        public string Bank_Code { get; set; }
        // [Required(ErrorMessage = "Bank Type is Required")]
        public string BNKTYPE_ID { get; set; }
        public string BNKTYPE_NAME { get; set; }
        [Required(ErrorMessage = "Rating Name is Required")]
        public string RATING_ID { get; set; }
        public string RATING_NAME { get; set; }
        [Required(ErrorMessage = "Status is Required")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Kindly Enter Correct Status")]
        public string BANK_STATUS { get; set; }
        // [Required(ErrorMessage = "Bank Address Line 1 is required")]
        public string BANK_ADDRESSLINE1 { get; set; }
        //[Required(ErrorMessage = "Bank Address Line 2 is required")]
        public string BANK_ADDRESSLINE2 { get; set; }
          [Required(ErrorMessage = "Country Name is required")]
        public string COUNTRY_ID { get; set; }
        public string COUNTRY_NAME { get; set; }
         [Required(ErrorMessage = "State Name is required")]
        public string STATE_ID { get; set; }
        public string STATE_NAME { get; set; }
        [Required(ErrorMessage = "City Name is required")]
        public string CITY_ID { get; set; }
        public string CITY_NAME { get; set; }
        // [Required(ErrorMessage = "Country Name is required")]
        
////        ^[0-9]*$
//        [StringLength(13, ErrorMessage = "The must contains 7 digits", MinimumLength = 7)]
//        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Kindly Enter Numeric Characters")]
        public string BANK_PHONES { get; set; }
        // [Required(ErrorMessage = "Country Name is required")]

        [StringLength(13, ErrorMessage = "The must contains 7 digits", MinimumLength = 7)]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Kindly Enter Numeric Characters")]
        public string BANK_FAX { get; set; }
        public string BANK_FAX_UPDATED { get; set; }
        //[Required(ErrorMessage = "Email is required")]
        //[RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
        //ErrorMessage = "Please Enter Correct Email Address")]
        public string BANK_EMAIL { get; set; }
        // [Required(ErrorMessage = "Country Name is required")]
        public string BANK_WEBSITE { get; set; }
        //// [Required(ErrorMessage = "Country Name is required")]
        //[RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "Kindly Enter Name In Correct Format")]
        public string BANK_CONTACTPERSONNAME { get; set; }
        //// [Required(ErrorMessage = "Country Name is required")]
        //[StringLength(13, ErrorMessage = "The must contains 7 digits", MinimumLength = 7)]
        //[RegularExpression(@"^[0-9]*$", ErrorMessage = "Kindly Enter Numeric Characters")]
        public string BANK_CONTACTPERSONMOBILE { get; set; }
        // [Required(ErrorMessage = "Country Name is required")]
    //[RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
    //ErrorMessage = "Please Enter Correct Email Address")]
        public string BANK_CONTACTPERSONEMAIL {get;set;}

    public string BANK_isdefault { get; set; }
    public bool BANK_isdefault_bool { get; set; }
        
        public List<Bank_Setup> bank_setup_list {get;set;}
        public List<Bank_Setup> rating_list {get;set;}
        public List<Bank_Setup> bank_type_list {get;set;}
        public List<Bank_Setup> city_list { get;set;}
        public List<Bank_Setup> state_list { get; set;}
        public List<Bank_Setup> country_list { get; set;}

    }
    public class Bank_Setup_pdf
    {      
        public string Name { get; set; }        
        public string ShortName { get; set; }                
        public string BankCode { get; set; }       
        public string BankType { get; set; }        
        public string Rating { get; set; }           
        public string Status { get; set; }
        //public string CreatedBy { get; set; }
        //public string CreatedOn { get; set; }
        //public string ModifiedBy { get; set; }
        //public string ModifiedOn { get; set; }    
    }

    
    public class Branch_Setup
    {
        public string BBR_ID { get; set; }
        [Required(ErrorMessage = "Bank Name is Required")]
        public string BANK_ID { get; set; }
        public string BANK_NAME { get; set; }
        [Required(ErrorMessage = "Branch Name is Required")]
        public string BBR_NAME { get; set; }
        //[Required(ErrorMessage = "Branch Code is Required")]
        [StringLength(4, ErrorMessage = "The Code must contains 4 characters", MinimumLength = 4)]
        public string BBR_CODE { get; set; }
        public string BBR_ISISLAMIC { get; set; }
        public bool BBR_ISISLAMIC_BOOL { get; set; }
      // [Required(ErrorMessage = "Branch swift Code is Required")]
        [StringLength(3,ErrorMessage = "The Code must contains 3 characters", MinimumLength = 3)]
        [RegularExpression(@"^[0-9]*$",ErrorMessage = "Enter Only numeric charaters")]        
       // ^[0-9]*$
        public string BBR_SWIFTCODE { get; set; }
        [Required(ErrorMessage = "Branch status is Required")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Kindly Enter Correct Status")]
        public string BBR_STATUS { get; set; }
        public string BBR_ADDRESSLINE1 { get; set; }
        public string BBR_ADDRESSLINE2 { get; set; }
        public string COUNTRY_ID { get; set; }
        public string COUNTRY_NAME { get; set; }
        public string STATE_ID { get; set; }
        public string STATE_NAME { get; set; }
        public string CITY_ID { get; set; }
        public string CITY_NAME { get; set; }
        [StringLength(13, ErrorMessage = "The must contains 7 digits", MinimumLength = 7)]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Kindly Enter Numeric Characters")]
        public string BBR_PHONES { get; set; }
        [StringLength(13, ErrorMessage = "The must contains 7 digits", MinimumLength = 7)]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Kindly Enter Numeric Characters")]
        public string BBR_FAX { get; set; }
        public string BBR_EMAIL { get; set; }
        [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "Kindly Enter Name In Correct Format")]
        public string BBR_CONTACTPERSONNAME { get; set; }
        [StringLength(13, ErrorMessage = "The must contains 7 digits", MinimumLength = 7)]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Kindly Enter Numeric Characters")]
        public string BBR_CONTACTPERSONMOBILE { get; set; }
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
        ErrorMessage = "Please Enter Correct Email Address")]
        public string BBR_CONTACTPERSONEMAIL { get; set; }

        public bool BBR_isdefault_bool { get; set; }

        public string BBR_isdefault { get; set; }
     


        public List<Branch_Setup> branch_setup_list { get; set; }
        public List<Branch_Setup> bank_setup_list { get; set; }
        // public List<Branch_Setup>rating_list { get; set; }
        // public List<Branch_Setup>bank_type_list { get; set; }
        public List<Branch_Setup> city_list { get; set; }
        public List<Branch_Setup> state_list { get; set; }
        public List<Branch_Setup> country_list { get; set; }
    }

    public class Branch_Setup_pdf
    {       
        public string BankName{ get; set; }       
        public string Name{ get; set; }      
        public string BranchCode{ get; set; }
        public string IsIslamaic{ get; set; }      
        public string SwiftCode{ get; set; }      
        public string Status{ get; set; }
        public string Address { get; set; }
    }    
    public class Fund_Setup
    {
        public string FUND_ID { get; set; }
        public string FUND_TID { get; set; }
        public string FUND_NAME { get; set; }
        public string FUND_SHORTNAME { get; set; }
       [Required(ErrorMessage = "Kindly provide Fund")]
        public string FUND_COMPID { get; set; }
         
        public string FUND_COMPName { get; set; }
        public string FUND_STATUS { get; set; }
        public List<Fund_Setup> Fund_setup_list { get; set; }
        public List<Fund_Setup> Fund_tid_list { get; set; }
        public List<Fund_Setup> Fund_compid_list { get; set; }
        public Dictionary<string, string> fund_dictionary = new Dictionary<string, string>();

    }
    public class Fund_Setup_pdf
    {
        public string Serial {get;set;}
        public string Name {get;set;}
        public string ShortName {get;set;}
        public string Status {get;set;}
        public string CreatedBy {get; set; }
        public string CreatedOn {get; set; }
        public string ModifiedBy {get; set; }
        public string ModifiedOn {get; set; }
        public List<Fund_Setup_pdf> Fund_setup_list { get; set; }    
    }
 
    public class AccountPurpose_Setup
    {
        public string ACCPUR_ID { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string ACCPUR_NAME { get; set; }
        [Required(ErrorMessage = "Short Name is Required")]
        public string ACCPUR_SHORTNAME { get; set; }
        [Required(ErrorMessage = "Status is Required")]
        public string ACCPUR_STATUS { get; set; }
        public string ACCPUR_ISDEFAULT { get; set; }
        public bool ACCPUR_ISDEFAULT_BOOL { get; set; }
        public List<AccountPurpose_Setup> accountpurpose_list { get; set; }

    }

    public class AccountPurpose_Setup_pdf
    {
        ///////////////////////////////////////// for pdf ////////////////////////////
        public string Serial { get; set; }
        public string Name { get; set; }
        public string Shortname { get; set; }
        public string Status { get; set; }
        public string IsDefault { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public List<AccountPurpose_Setup_pdf> accountpurpose_list { get; set; }
        ///////////////////////////////////////// for pdf ////////////////////////////
    }

    public class Fundbankaccount_Setup
    {
        public string FBACC_ID { get; set; }
        [Required(ErrorMessage = "Required")]
        public string FUND_ID { get; set; }
        public string FUND_NAME { get; set; }
        [Required(ErrorMessage = "Required")]
        public string BANK_ID { get; set; }
        public string BANK_NAME { get; set; }
        [Required(ErrorMessage = "Required")]
        public string BBR_ID { get; set; }
        public string BBR_NAME { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ACCTYPE_ID { get; set; }
        public string ACCTYPE_NAME { get; set; }
        public string ACCPUR_NAME { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ACCPUR_ID { get; set; }
        [Required(ErrorMessage = "Required")]
        public string PBAS_ID { get; set; }
        public string PBAS_NAME { get; set; }
        [Required(ErrorMessage = "Required")]
        public string PFRQ_ID { get; set; }
        public string PFRQ_NAME { get; set; }

        [Required(ErrorMessage = "Required")]
        public string SLAB_ID { get; set; }
        public string SLAB_NAME { get; set; }

        
        public string FBACC__ISPOOLED { get; set; }
        public bool FBACC__ISPOOLED_BOOL { get; set; }
        [Required(ErrorMessage = "Number is required")]
        public string FBACC_NUMBER { get; set; }
        [Required(ErrorMessage = "Title is required")]
        
        public string FBACC_TITLE { get; set; }
        
        public string FBACC_OPENINGDATE { get; set; }
        
        public string FBACC_ISCLOSED { get; set; }
        public bool FBACC_ISCLOSED_BOOL { get; set; }
        
        public string FBACC_CLOSINGDATE { get; set; }
        //[Required(ErrorMessage = "Amount is required")]
        //[StringLength(9, ErrorMessage = "The Code must contains 9 characters", MinimumLength = 1)]
        //[RegularExpression(@"^[1-9][0-9]*$", ErrorMessage="Enter value in correct format")]
        public string FBACC_TDEBITACCOUNT {get;set;}
        
        
        //[Required(ErrorMessage = "Amount is required")]
        //[StringLength(9, ErrorMessage = "The Code must contains 4 characters", MinimumLength = 1)]
        //[RegularExpression(@"^[1-9][0-9]*$", ErrorMessage = "Enter Only numeric charaters")]
        public string FBACC_TCREDITACCOUNT { get; set; }
        [Required(ErrorMessage = "Status is Required")]
        public string FBACC_STATUS { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string FBACC_COMPID { get; set; }
      
      //  [Display(Name = "closing date")]
       [DataType(DataType.Date)]
        //  [Required(ErrorMessage = "date is Required")]
       [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime Closing_Date { get; set; }

      

      //  [Display(Name = "opening date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "date is Required")]
       // [Compare]
       // [Compare(ErrorMessage = "Proivde correct opening date")]
      //  [System.Web.Mvc.CompareAttribute(Closing_Date)] 
      //[DateValidation( ErrorMessage = "Sorry, the date can't be later than today's date")]

      //  [IsDateAfterAttribute("Closing_Date", true,ErrorMessageResourceName = "PeriodErrorMessage",ErrorMessage=null)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

            public DateTime Opening_Date { get; set; }

      //  [Compare("T",ErrorMessage = "Provide correct opening date")] 
     //   public string checkdate {get;set;}

        public string FBACCID_tran_detail { get; set; }

        public string dll_bank { get; set; }


        //public string FBACC_ID {get;set;}
        //public string FBACC_ID {get;set;}
        public List<Fundbankaccount_Setup> fundbankaccount_setup_list { get; set; }
        public List<Fundbankaccount_Setup> fund_list { get; set; }
        public List<Fundbankaccount_Setup> bank_list { get; set; }
        public List<Fundbankaccount_Setup> branch_list { get; set; }
        public List<Fundbankaccount_Setup> acctype_list { get; set; }
        public List<Fundbankaccount_Setup> accpur_list { get; set; }
        public List<Fundbankaccount_Setup> pbas_list { get; set; }
        public List<Fundbankaccount_Setup> pfrq_list { get; set; }
        public List<Fundbankaccount_Setup> slab_list { get; set; }


    }

    public class Fundbankaccount_Setup_pdf
    {
       
      //  public string Serial { get; set; }
        public string FundName { get; set; }
        public string BankName { get; set; }        
        public string BranchName { get; set; }        
        public string AcctypeName { get; set; }
        public string AccpurName { get; set; }       
        public string PbasName { get; set; }       
        public string PfrqName { get; set; }
        public string IsPooled { get; set; }
        //public bool FBACC__ISPOOLED_BOOL { get; set; }       
        public string AccNumber { get; set; }       
        public string Title { get; set; }
        public string OpeningDate { get; set; }
        public string IsClosed { get; set; }
       // public bool FBACC_ISCLOSED_BOOL { get; set; }
        public string ClosingDate { get; set; }
        //public string Debit { get; set; }        
        //public string Credit { get; set; }        
        public string Status { get; set; }
        //public string FBACCID_tran_detail { get; set; }

    }    
    public class ProfitFrequency_Setup
    {

        public string PFRQ_ID { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string PFRQ_NAME { get; set; }
        [Required(ErrorMessage = "Short Name is Required")]
        public string PFRQ_SHORTNAME { get; set; }
        [Required(ErrorMessage = "Status is Required")]
        public string PFRQ_STATUS { get; set; }
        public string PFRQ_ISDEFAULT { get; set; }
        public bool PFRQ_ISDEFAULT_BOOL { get; set; }
        public List<ProfitFrequency_Setup> profit_frequency_list { get; set; }

    }
    public class ProfitFrequency_Setup_pdf
    {
        public string Serial { get; set; }
        public string Name { get; set; }
        public string Shortname { get; set; }
        public string Status { get; set; }
        public string IsDefault { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public List<ProfitFrequency_Setup_pdf> profit_frequency_list{get;set;}
    }
   
    public class ProfitBasis_Setup
    {

        public string PBAS_ID { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string PBAS_NAME { get; set; }
        [Required(ErrorMessage = "Short Name is Required")]
        public string PBAS_SHORTNAME { get; set; }
        [Required(ErrorMessage = "Status is Required")]
        public string PBAS_STATUS { get; set; }
        public string PBAS_ISDEFAULT { get; set; }
        public bool PBAS_ISDEFAULT_BOOL { get; set; }
        public List<ProfitBasis_Setup> profit_basis_list { get; set; }

    }

    public class ProfitBasis_Setup_pdf
    {

        public string Serial { get; set; }
        public string Name { get; set; }
        public string Shortname { get; set; }
        public string Status { get; set; }
        public string IsDefault { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public List<ProfitBasis_Setup_pdf> profit_basis_list { get; set; }
    
    }

    
    public class BalamountSlabs_Setup
    {

        public string SLAB_ID { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string SLAB_NAME { get; set; }
        [Required(ErrorMessage = "Short Name is Required")]
        public string SLAB_SHORTNAME { get; set; }
        [Required(ErrorMessage = "Status is Required")]
        public string SLAB_STATUS { get; set; }
        public string SLAB_ISDEFAULT { get; set; }
        public bool SLAB_ISDEFAULT_BOOL { get; set; }
        [Required(ErrorMessage = "Amount is Required")]

       // [StringLength(10, ErrorMessage = "The Code must contains 10 characters", MinimumLength = 1)]
        [RegularExpression(@"^[1-9][0-9]*$", ErrorMessage = "Enter value in correct format")]

        [IsDateAfterAttribute1("SLAB_AMOUNTTO", true, ErrorMessageResourceName = "PeriodErrorMessage", ErrorMessage = null)]
        public string SLAB_AMOUNTFROM { get; set; }
        [Required(ErrorMessage = "Amount is Required")]
        //[StringLength(10, ErrorMessage = "The Code must contains 10 characters", MinimumLength = 1)]
        [RegularExpression(@"^[1-9][0-9]*$", ErrorMessage = "Enter value in correct format")]
        public string SLAB_AMOUNTTO { get; set; }
         [Required(ErrorMessage = " Bank is Required")]
        public string BANK_ID { get; set; }
        public string BANK_NAME { get; set; }
        
        public List<BalamountSlabs_Setup> balamounts_slab_list { get; set; }
        public List<BalamountSlabs_Setup> bank_list { get; set; }


    }
    public class BalamountSlabs_Setup_pdf
    {
        ///////////////////////////////////////// for pdf ////////////////////////////
        public string Serial { get; set; }
        public string Name { get; set; }
        public string Shortname { get; set; }
        public string Status { get; set; }
        public string IsDefault { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public List<AccountPurpose_Setup_pdf> balamounts_slab_list { get; set; }
        public string AmountFrom { get; set; }
        public string AmountTO { get; set; }

        ///////////////////////////////////////// for pdf ////////////////////////////
    
    }
    
    
    public class ProfitRate_Setup
    {
        public string PTRATE_ID { get; set; }
        public string SLAB_ID { get; set; }
        public string BANK_ID { get; set; }
        public string BBR_ID { get; set; }
        public string ACCTYPE_ID { get; set; }
        public string ACCPUR_ID { get; set; }
      //  [Required(ErrorMessage = "Date is required")]
        public string PTRATE_EFFECTIVEFROM { get; set; }
       // [Required(ErrorMessage = "Date is required")]
       
        public string PTRATE_EFFECTIVETO { get; set; }
        public string PTRATE_STATUS { get; set; }
        public string PTRATE_COMPID { get; set; }
        [Required(ErrorMessage = "required")]
        //[StringLength(3, ErrorMessage = "Kindly provide values in correct format", MinimumLength = 1)]
        //[RegularExpression(@"^[0-9]*$", ErrorMessage = "Enter Only numeric charaters")]
        //[StringLength(4, ErrorMessage = "Must be of two digits", MinimumLength = 1)]
        //[RegularExpression(@"^[1-9][0-9]*$", ErrorMessage = "Enter value in correct format")]
        [RegularExpression(@"[+-]?([0-9]*[.])?[0-9]+", ErrorMessage = "Enter value in correct format")]
       

        public string PTRATE_PERCENT { get; set; }
        public string SLAB_NAME { get; set; }
        public string BANK_NAME { get; set; }
        public string BBR_NAME { get; set; }
        public string ACCTYPE_NAME { get; set; }
        public string ACCPUR_NAME { get; set; }
        public string COMP_NAME { get; set; }

       // [Required(ErrorMessage = "date is Required")]
        //[Display(Name = "EFFECTIVEFROM")]
        [DataType(DataType.Date)]
       // [IsDateAfterAttribute("EFFECTIVETO", true, ErrorMessageResourceName = "PeriodErrorMessage", ErrorMessage = null)]        
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            
            public DateTime EFFECTIVEFROM { get; set; }
        
        //[Display(Name = "EFFECTIVETO")]
        [DataType(DataType.Date)]
      //  [Required(ErrorMessage = "date is Required")]        

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

            public DateTime EFFECTIVETO { get; set; }


        public List<ProfitRate_Setup> fund_list { get; set; }
        public List<ProfitRate_Setup> bank_list { get; set; }
        public List<ProfitRate_Setup> branch_list { get; set; }
        public List<ProfitRate_Setup> acctype_list { get; set; }
        public List<ProfitRate_Setup> accpur_list { get; set; }
        public List<ProfitRate_Setup> slab_list { get; set; }
        public List<ProfitRate_Setup> profit_rate_list { get; set; }


    }

    public class ProfitRate_Setup_pdf
    {
        public string Serial { get; set; }      
        public string EffectiveFrom { get; set; }
        public string EffectiveTo { get; set; }
        public string Status { get; set; }     
        public string Percent { get; set; }
        public string SlabName { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string AccountTypeName { get; set; }
        public string AccountPurposeName { get; set; }
        public string CompanyName { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }



        public List<ProfitRate_Setup_pdf> profit_rate_list { get; set; }

    }


    public class Trandetail
    {

        public string txnd_id { get; set; }
        public string txnmstr_id { get; set; }
        public string fbacc_id { get; set; }
        public string fund_id { get; set; }
        public string fund_name { get; set; }
        public string bank_name { get; set; }
        public string bbr_name { get; set; }
        public string fbacc_number { get; set; }
        public string txnd_type { get; set; }
        public string txnd_amount { get; set; }
        public string ptrate_percent { get; set; }
        public string profit { get; set; }
        public string txnd_l_amount { get; set; }
        public string value_date { get; set; }
        public string txnd_status { get; set; }
        public string txnd_postedon { get; set; }
        public string last_postDate { get; set; }
        public string postDate { get; set; }
        public string last_valueDate { get; set; }

        public List<Trandetail> trandetail_list { get; set; }



    }

    public class trandetail_fund
    {

        public string search_option { get; set; }
        public string BANK_ID { get; set; }
        public string BANK_NAME { get; set; }

        public string FUND_ID { get; set; }
        public string FUND_NAME { get; set; }
        public string FUND_BANK_ACC_ID { get; set; }
        public string post_date_string { get; set; }

        [Display(Name = "Value Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "date is Required")]
        public DateTime Val_Date { get; set; }


        [Display(Name = "Transfer Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "date is Required")]
        public DateTime Transfer_Date { get; set; }



        public List<trandetail_fund> trandetail_fund_list { get; set; }

        public List<trandetail_fund> FUND_OPTION_LIST { get; set; }
        public List<trandetail_fund> BANK_OPTION_LIST { get; set; }
        /// <summary>
        /// /////////////////
        /// </summary>
        public string tf_txnd_id { get; set; }
        public string tf_txnmstr_id { get; set; }
        public string tf_fbacc_id { get; set; }
        public string tf_fund_id { get; set; }
        public string tf_fund_name { get; set; }
        public string tf_bank_name { get; set; }
        public string tf_bbr_name { get; set; }
        public string tf_fbacc_number { get; set; }
        public string tf_txnd_type { get; set; }
        public string tf_txnd_amount { get; set; }
        public string tf_ptrate_percent { get; set; }
        public string tf_profit { get; set; }
        public string tf_txnd_l_amount { get; set; }
        public string tf_value_date { get; set; }
        public string tf_txnd_status { get; set; }
        public string tf_txnd_postedon { get; set; }
        public string tf_last_postDate { get; set; }
        public string tf_postDate { get; set; }
        public string tf_last_valueDate { get; set; }


    }

    public class Country
    {
       public string id{get;set;}
       [Required(ErrorMessage = "Required")]
       [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "Kindly Enter Correct  Name")]
       public string name{get;set;}
       [Required(ErrorMessage = "Required")]
       [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "Kindly Enter Correct Short Name")]
       public string sname{get;set;}
        //public string id{get;set;}

       public string COUNTRY_STATUS { get; set; }
        
        public List<Country> country_list { get; set; }
    }
    public class State
    {
        public string id {get;set;}
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "Kindly Enter Correct  Name")]
        public string name {get;set;}
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "Kindly Enter Correct Short Name")]
        public string sname {get;set;}
        public string country_id{get;set;}
        public string country_name {get;set;}
        public string STATE_STATUS { get; set; }        
        public List<State> state_list{get;set;}
        public List<State> country_list { get; set; }    
    }
    public class City
    {
        public string id {get;set;}
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "Kindly Enter Correct  Name")]
        public string name {get;set;}
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "Kindly Enter Correct Short Name")]
        public string sname {get;set;}
        public string country_id {get;set;}
        public string country_name {get;set;}
        public string state_id {get;set;}
        public string state_name {get;set;}
        public string CITY_STATUS { get; set; }        
        public List<City>city_list {get;set;}
        public List<City>state_list {get; set;}
        public List<City>country_list {get;set;}    
    }


    public class Process
    {
  public string ProcessType {get;set;}

  [Display(Name = "Process Date")]
  [DataType(DataType.Date)]
  [Required(ErrorMessage = "Date is Required")]        
  public DateTime ProcessDate {get;set;}
  public string Purpose {get;set;}
  public List<Process> Process_list {get;set;}
  public List<trandetail_fund> Process_TranDetail_Fund { get; set; }    
    }

    public class Profit_Accrual_Report 
    {

        [Display(Name = "Process Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date is Required")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date is Required")]
        public DateTime EndDate { get; set; }
        public string Purpose { get; set; }

        public string FBACC_NUMBER { get; set; }
        public string search_option { get; set; }

        public List<Process> Process_list { get; set; }
        public List<trandetail_fund> Process_TranDetail_Fund { get; set; }    
   
    }

    public class Bank_Balance_Report
    {

        [Display(Name = "Process Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date is Required")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date is Required")]
        public DateTime EndDate { get; set; }
        public string Purpose { get; set; }

        public string FBACC_NUMBER { get; set; }
        public string search_option { get; set; }

        public List<Process> Process_list { get; set; }
        public List<trandetail_fund> Process_TranDetail_Fund { get; set; }

    }





    
    
    
    
    
    


    //public sealed class IsDateAfterAttribute : ValidationAttribute, IClientValidatable
    //{
    //    private readonly string testedPropertyName;
    //    private readonly bool allowEqualDates;

    //    public IsDateAfterAttribute(string testedPropertyName, bool allowEqualDates = false)
    //    {
    //        this.testedPropertyName = testedPropertyName;
    //        this.allowEqualDates = allowEqualDates;
    //    }

    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {

         
            
    //        var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.testedPropertyName);

          
            
    //        if (propertyTestedInfo == null)
    //        {
    //            return new ValidationResult(string.Format("unknown property {0}", this.testedPropertyName));
    //        }


    //        var propertyTestedValue = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

    //        if ((DateTime)propertyTestedValue == DateTime.Parse("01/01/0001"))
    //        {
    //            return ValidationResult.Success;
    //        }



    //        if (value == null || !(value is DateTime))
    //        {
    //            return ValidationResult.Success;
    //        }

    //        if (propertyTestedValue == null || !(propertyTestedValue is DateTime))
    //        {
    //            return ValidationResult.Success;
    //        }

    //        // Compare values
    //        if ((DateTime)value <= (DateTime)propertyTestedValue)
    //        {
    //            if (this.allowEqualDates && value == propertyTestedValue)
    //            {
    //                return ValidationResult.Success;
    //            }
    //            else if ((DateTime)value < (DateTime)propertyTestedValue)
    //            {
    //                return ValidationResult.Success;
    //            }
    //        }

          


    //        return new ValidationResult("kindly provide appropriate date");
    //    }

    //    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    //    {
    //        var rule = new ModelClientValidationRule
    //        {
    //            ErrorMessage = "kindly provide appropriate date",
    //            ValidationType = "isdateafter"
    //        };
    //        rule.ValidationParameters["propertytested"] = this.testedPropertyName;
    //        rule.ValidationParameters["allowequaldates"] = this.allowEqualDates;
    //        yield return rule;
    //    }



    //}


    public sealed class IsDateAfterAttribute1 : ValidationAttribute, IClientValidatable
    {
        private readonly string testedPropertyName;
        private readonly bool allowEqualDates;

        public IsDateAfterAttribute1(string testedPropertyName, bool allowEqualDates = false)
        {
            this.testedPropertyName = testedPropertyName;
            this.allowEqualDates = allowEqualDates;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.testedPropertyName);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.testedPropertyName));
            }



            var propertyTestedValue = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (propertyTestedValue == null)
            {
                return ValidationResult.Success;
            }



            Int64 v = Int64.Parse((string)value);
            Int64 ptv = Int64.Parse((string)propertyTestedValue);


            if (v == null || !(v is Int64))
            {
                return ValidationResult.Success;
            }

            if (ptv == null || !(ptv is Int64))
            {
                return ValidationResult.Success;
            }

            // Compare values
            if (v <= ptv)
            {
                if (this.allowEqualDates && value == propertyTestedValue)
                {
                    return ValidationResult.Success;
                }
                else if (v < ptv)
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult("Kindly provide appropriate amount");
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = "Kindly provide appropriate amount",
                ValidationType = "isdateafter"
            };
            rule.ValidationParameters["propertytested"] = this.testedPropertyName;
            rule.ValidationParameters["allowequaldates"] = this.allowEqualDates;
            yield return rule;
        }



    }





}



