using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagementBLL.Services.Interface
{
    public interface IAppointmentService
    {
        bool IsDoctorWorking(int doctorId, DateTime date);
    }
}
