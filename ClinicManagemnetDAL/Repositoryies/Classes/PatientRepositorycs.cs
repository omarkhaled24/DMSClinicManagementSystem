//using ClinicManagemnetDAL.Data.Contexts;
//using ClinicManagemnetDAL.Models;
//using ClinicManagemnetDAL.Repositoryies.Interface;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ClinicManagemnetDAL.Repositoryies.Classes
//{
//    internal class PatientRepositorycs : IPatientRepositorycs
//    {

//        public int Add(Patient patient)
//        { 
//            clinicDbContext.Patients.Add(patient);
//            return clinicDbContext.SaveChanges();
//        }

//        public int Delete(int Id)
//        {
//            var patient = clinicDbContext.Patients.Find(Id);
//            if (patient is null) return 0;
//            clinicDbContext.Patients.Remove(patient);
//            return clinicDbContext.SaveChanges();

//        }

//        public IEnumerable<Patient> GetAll() => clinicDbContext.Patients.ToList();

//        public Patient? GetById(int id) => clinicDbContext.Patients.Find(id);

//        public int update(Patient patient)
//        {
//            clinicDbContext.Patients.Update(patient);
//            return clinicDbContext.SaveChanges();
//        }
//    }
//}
