using ClinicManagementBLL.Services.Interface;
using ClinicManagemnetDAL.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagementBLL.Services.Classes
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ClinicDbContext clinicDbContext;

        public AppointmentService(ClinicDbContext clinicDbContext)
        {
            this.clinicDbContext = clinicDbContext;
        }
        public bool IsDoctorWorking(int doctorId, DateTime date)
        {
            DayOfWeek selectedDay = date.DayOfWeek;

            var schedule = clinicDbContext.Schedules
                .FirstOrDefault(s => s.DoctorId == doctorId && s.DayOfWeek == selectedDay);

            if (schedule == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
