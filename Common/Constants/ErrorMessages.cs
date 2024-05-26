using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Constants
{
    public static class ErrorMessages
    {
        public const string USER_NOT_FOUND = "Пользовать не был найден";
        public const string APPLICANT_NOT_FOUND = "Пользовать не является абитуриентом";
        public const string MANAGER_NOT_FOUND = "Не найден нужный менеджер";
        public const string APPLICATION_NOT_FOUND = "Не удалось найти заявку";
        public const string INVALID_PASSPORT_SERIES = "Неправильная серия паспорта";
        public const string INVALID_PASSPORT_NUMBER = "Неправильный номер паспорта";
        public const string INVALID_PROGRAMS = "Ошибка в выбранных программах";
        public const string SCANS_LIST = "Пожалуйста добавьте сканы для документа";
        public const string FACULTY_NOT_FOUND = "Факультет не был найден";
        public const string EDUCATION_DOCUMENT_TYPE_NOT_FOUND = "Документ об образовании не был найден";
        public const string PROGRAM_NOT_FOUND = "Программа не была найден";
        public const string EDUCATIOM_LEVEL_NOT_FOUND = "Уровень образования не был найден";
        public const string IMPORT = "Произошла ошибка при импорте";
        public const string REQUEST = "Произошла ошибка при запросе";
        public const string EMAIL_DUBLICATE = "Пользователь с таким Email уже существует";
        public const string INVALID_CREDINTAILS = "Неверный Email или пароль";
        public const string SESSION = "Пользователь не имеет сессии на данном устройстве, попробуй войти заново";
        public const string SERVER_ERROR = "Произошла ошибка сервера";
        public const string DUBLICATE_TOKEN = "Данный токен уже был использован, все сессии заблокированы";
        public const string UNAUTHORIZE = "Пожалуйста войдите заново";
    }
}
