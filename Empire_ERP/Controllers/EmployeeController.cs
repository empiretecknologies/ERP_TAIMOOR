using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class EmployeeController : BaseController
    {
        public IEmployeeService _employeeService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public EmployeeController(IEmployeeService employeeService, IMenuService menuService, IWebHostEnvironment hostingEnvironment,IBaseService baseService) : base(menuService,baseService)
        {
            _employeeService = employeeService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            ViewBag.Permissions = CommonHelper.GetValues(HttpContext).RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).MenuID);

            var data = GetEmpDataByOfferLetter();
            ViewBag.EmpDataByOfferLetter = data.Value;
            return View();
        }

        [HttpPost]
        public JsonResult SaveMainOtherInfo(Employee employee)
        {
            try
            {
                var data = _employeeService.Save(employee, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(_catchMessage);
            }
        }

        [HttpPost]
        public JsonResult Delete(int code, int actCode)
        {
            try
            {
                var data = _employeeService.Delete(code, actCode, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(_catchMessage);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage()
        {

            MyHttpResponseMessage response = new MyHttpResponseMessage();
            var uniqueFileName = "";
            try
            {
                IFormFile Image = Request.Form.Files[0];
                if (Image != null && Image.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Client", "EmployeeDocuments");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    string extension = Path.GetExtension(Image.FileName);
                    uniqueFileName = Guid.NewGuid().ToString().Substring(0, 25) + "_" + extension;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(fileStream);
                    }
                }
                response.msg = "File uploaded seccessfully.";
                response.msgType = 1;
                response.data = uniqueFileName;
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                response.msg = _catchMessage;
                response.msgType = 2;
            }

            return Json(response);
        }

        [HttpGet]
        public JsonResult GetEmployeeByEmployeeCode(int code)
        {
            try
            {
                var employeeData = _employeeService.GetEmployeeByEmployeeCode(code, CommonHelper.GetValues(HttpContext));
                return Json(new { employeeData = employeeData });
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(_catchMessage);
            }
        }

        [HttpGet]
        public JsonResult QuickSearchEmployee()
        {
            var data = _employeeService.QuickSearchEmployee(CommonHelper.GetValues(HttpContext));
            return Json(data);
        }

        [HttpGet]
        public JsonResult GetReligions()
        {
            try
            {
                var data = DropdownService.ReligionDropdown();
                return Json(new { data = data, msgType = 1 });
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { data = _catchMessage, msgType = 2 });
            }
        }

        [HttpGet]
        public JsonResult GetOfferLetters()
        {
            try
            {
                var data = DropdownService.OfferLetterDropdown();
                return Json(new { data = data, msgType = 1 });
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { data = _catchMessage, msgType = 2 });
            }
        }

        [HttpGet]
        public JsonResult GetDesignations()
        {
            try
            {
                var data = DropdownService.DesignationDropdown();
                return Json(new { data = data, msgType = 1 });
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { data = _catchMessage, msgType = 2 });
            }
        }

        [HttpGet]
        public JsonResult GetDepartments()
        {
            try
            {
                var data = DropdownService.DepartmentDropdown();
                return Json(new { data = data, msgType = 1 });
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { data = _catchMessage, msgType = 2 });
            }
        }

        [HttpGet]
        public JsonResult GetEmploymentTypes()
        {
            try
            {
                var data = DropdownService.EmploymentTypeDropdown();
                return Json(new { data = data, msgType = 1 });
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { data = _catchMessage, msgType = 2 });
            }
        }

        [HttpGet]
        public JsonResult GetShifts()
        {
            try
            {
                var data = DropdownService.ShiftDropdown();
                return Json(new { data = data, msgType = 1 });
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { data = _catchMessage, msgType = 2 });
            }
        }

        [HttpGet]
        public JsonResult GetBranches()
        {
            try
            {
                var data = DropdownService.BranchDropdown();
                return Json(new { data = data, msgType = 1 });
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { data = _catchMessage, msgType = 2 });
            }
        }

        [HttpGet]
        public JsonResult GetEducations()
        {
            try
            {
                var data = DropdownService.EducationDropdown();
                return Json(new { data = data, msgType = 1 });
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { data = _catchMessage, msgType = 2 });
            }
        }

        [HttpGet]
        public JsonResult GetEmpDataByOfferLetter()
        {
            List<dynamic> data = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = @"SELECT OL.TRAN_ID, C.FULL_NAME AS ENAME, C.EMAIL, C.PHONE AS CELL_NO, C.GENDER, C.DOB, C.CON_ADDRESS AS EMP_ADD, 
                                    C.EDUCATION_ID, OL.DEP_ID, OL.JOINING_DATE, J.BCODE FROM TBL_JOB_TYPE J
                                    LEFT OUTER JOIN TBL_CANDIDATES C ON C.JOB_ID = J.TRAN_ID
                                    LEFT OUTER JOIN TBL_INTERVIEW_SCH S ON S.CON_ID = C.TRAN_ID
                                    LEFT OUTER JOIN TBL_OFFER_LETTER OL ON OL.INT_ID = S.TRAN_ID
                                    WHERE J.DLT = 'T'";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var item = new
                                {
                                    TRAN_ID = reader["TRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TRAN_ID"]),
                                    ENAME = reader["ENAME"] == DBNull.Value ? "" : reader["ENAME"].ToString(),
                                    EMAIL = reader["EMAIL"] == DBNull.Value ? "" : reader["EMAIL"].ToString(),
                                    CELL_NO = reader["CELL_NO"] == DBNull.Value ? "" : reader["CELL_NO"].ToString(),
                                    GENDER = reader["GENDER"] == DBNull.Value ? "" : reader["GENDER"].ToString(),
                                    DOB = reader["DOB"] == DBNull.Value ? null : Convert.ToDateTime(reader["DOB"]).ToString("yyyy-MM-dd"),
                                    EMP_ADD = reader["EMP_ADD"] == DBNull.Value ? "" : reader["EMP_ADD"].ToString(),
                                    EDUCATION_ID = reader["EDUCATION_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["EDUCATION_ID"]),
                                    DEP_ID = reader["DEP_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DEP_ID"]),
                                    JOINING_DATE = reader["JOINING_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["JOINING_DATE"]).ToString("yyyy-MM-dd"),
                                    BCODE = reader["BCODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BCODE"])
                                };

                                data.Add(item);
                            }
                        }
                    }
                    conn.Close();
                }
            }

            return Json(data);
        }


        [HttpPost]
        public async Task<IActionResult> UploadThumbs()
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            var uniqueFileName = "";
            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    IFormFile uploadedFile = Request.Form.Files[0];

                    if (uploadedFile != null && uploadedFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Client", "Employee", "Thumbs");

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string fileExtension = Path.GetExtension(uploadedFile.FileName);

                        uniqueFileName = Guid.NewGuid().ToString().Substring(0, 25) + fileExtension;

                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await uploadedFile.CopyToAsync(fileStream);
                        }

                        response.msg = "Document uploaded successfully.";
                        response.msgType = 1;
                        response.data = $"/Client/Employee/Thumbs/{uniqueFileName}";
                    }
                    else
                    {
                        response.msg = "No file to upload or file is empty.";
                        response.msgType = 2;
                    }
                }
                else
                {
                    response.msg = "No file found in the request.";
                    response.msgType = 2;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += "<br/>" + ex.InnerException.Message;
                }
                response.msg = errorMessage;
                response.msgType = 2;
            }
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> UploadSigns()
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            var uniqueFileName = "";
            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    IFormFile uploadedFile = Request.Form.Files[0];

                    if (uploadedFile != null && uploadedFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Client", "Employee", "Signs");

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string fileExtension = Path.GetExtension(uploadedFile.FileName);

                        uniqueFileName = Guid.NewGuid().ToString().Substring(0, 25) + fileExtension;

                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await uploadedFile.CopyToAsync(fileStream);
                        }

                        response.msg = "Document uploaded successfully.";
                        response.msgType = 1;
                        response.data = $"/Client/Employee/Signs/{uniqueFileName}";
                    }
                    else
                    {
                        response.msg = "No file to upload or file is empty.";
                        response.msgType = 2;
                    }
                }
                else
                {
                    response.msg = "No file found in the request.";
                    response.msgType = 2;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += "<br/>" + ex.InnerException.Message;
                }
                response.msg = errorMessage;
                response.msgType = 2;
            }
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> SaveImage()
        {

            MyHttpResponseMessage response = new MyHttpResponseMessage();
            var uniqueFileName = "";
            try
            {
                IFormFile Image = Request.Form.Files[0];
                if (Image != null && Image.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images", "upload", "employees");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    uniqueFileName = Guid.NewGuid().ToString().Substring(0, 25) + "_.png";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(fileStream);
                    }
                }
                response.msg = "File uploaded successfully.";
                response.msgType = 1;
                response.data = $"{uniqueFileName}";
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                response.msg = _catchMessage;
                response.msgType = 2;
            }
            return Json(response);
        }
    }
}