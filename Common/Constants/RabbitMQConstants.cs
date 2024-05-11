using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Constants
{
    public static class RabbitMQQueue
    {
        public const string ADD_USER = "ADD_USER_QUEUE";
        public const string GET_PROFILE = "GET_PROFILE_QUEUE";
        public const string SYNCHRONIZE_PROFILE = "EDIT_PROFILE_QUEUE";
        public const string ADD_ROLE = "ADD_ROLE_QUEUE";
        public const string DELETE_ROLE = "DELETE_ROLE_QUEUE";


        public const string ADMISSION_ADD_USER = "ADMISSION_ADD_USER_QUEUE";
        public const string ADMISSION_EDIT_USER = "ADMISSION_EDIT_USER_QUEUE";

        public const string APPLICANT_REGISTER = "APPLICANT_REGISTER_QUEUE";
        public const string GET_PASSPORT = "GET_PASSPORT_QUEUE";
        public const string GET_EDUCATION_DOCUMENT = "GET_EDUCATION_DOCUMENT_QUEUE";
        public const string GET_APPLICANT_DATA = "GET_APPLICANT_DATA_QUEUE";
        public const string EDIT_APPLICANT_DATA = "EDIT_APPLICANT_DATA_QUEUE";
    }
}
