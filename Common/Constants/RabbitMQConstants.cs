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
        public const string LOGIN = "LOGIN_QUEUE";
        public const string LOGOUT = "LOGOUT_QUEUE";
        public const string EDIT_PROFILE = "EDIT_PROFILE_QUEUE";

        public const string ADMISSION_ADD_USER = "ADMISSION_ADD_USER_QUEUE";
        public const string ADMISSION_EDIT_USER = "ADMISSION_EDIT_USER_QUEUE";

        public const string APPLICANT_REGISTER = "APPLICANT_REGISTER_QUEUE";
        public const string GET_PASSPORT = "GET_PASSPORT_QUEUE";
        public const string GET_EDUCATION_DOCUMENT = "GET_EDUCATION_DOCUMENT_QUEUE";
        public const string GET_APPLICANT_DATA = "GET_APPLICANT_DATA_QUEUE";
        public const string EDIT_APPLICANT_DATA = "EDIT_APPLICANT_DATA_QUEUE";

        public const string NOTIFICATION_SEND_EMAIL = "NOTIFICATION_SEND_EMAIL_QUEUE";

        public const string GET_IMPORTING_STATUSES = "GET_IMPORTING_STATUSES_QUEUE";
        public const string IMPORT_PROGRAMS = "IMPORT_PROGRAMS_QUEUE";
        public const string IMPORT_FACULTIES = "IMPORT_FACULTIES_QUEUE";
        public const string IMPORT_EDUCATION_DOCUMENTS = "IMPORT_EDUCATION_DOCUMENTS_QUEUE";
        public const string IMPORT_EDUCATION_LEVELS = "IMPORT_EDUCATION_LEVELS_QUEUE";
        public const string ADD_PROGRAM = "ADD_PROGRAM_QUEUE";
        public const string ADD_EDUCATION_DOCUMENT_TYPE = "ADD_EDUCATION_DOCUMENT_TYPE_QUEUE";
        public const string ADD_FACULTY = "ADD_FACULTY_QUEUE";
        public const string ADD_EDUCATION = "ADD_EDUCATION_QUEUE";
        public const string SYCHRONIZE_PROGRAMS = "SYCHRONIZE_PROGRAMS_QUEUE";
        public const string SYCHRONIZE_EDUCATION_DOCUMENT_TYPES = "SYCHRONIZE_EDUCATION_DOCUMENT_TYPES_QUEUE";
        public const string SYCHRONIZE_FACULTIES = "SYCHRONIZE_FACULTIES_QUEUE";
        public const string SYCHRONIZE_EDUCATIONS = "SYCHRONIZE_EDUCATIONS_QUEUE";
        public const string GET_MANAGERS = "GET_MANAGERS_QUEUE";

        public const string GET_ALL_PROGRAMS = "GET_ALL_PROGRAMS_QUEUE";
        public const string GET_FACULTIES = "GET_FACULTIES_QUEUE";

        public const string GET_APPLICATIONS = "GET_APPLICATIONS_QUEUE";
        public const string GET_APPLICATION = "GET_APPLICATION_QUEUE";

        public const string REFRESH_GUIDE = "REFRESH_GUIDE_QUEUE";
    }
}
