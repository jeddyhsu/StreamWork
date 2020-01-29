using System.Linq;

namespace StreamWork.HelperClasses
{
    public class FollowingHelperFunctions //All helper functions that have to with students following tutors
    {
        public string AddToListOfFollowedTutors(string id, string listOfFollwedTutors) //for students
        {
            if (listOfFollwedTutors == null)
                listOfFollwedTutors = id;
            else
                listOfFollwedTutors += "|" + id;

            return listOfFollwedTutors;
        }

        public string RemoveFromListOfFollowedTutors(string id, string listOfFollwedTutors) //for students
        {
            if (id != null && listOfFollwedTutors != null)
            {
                if (!listOfFollwedTutors.Contains('|'))
                    return null;

                var list = listOfFollwedTutors.Split("|").ToList();

                foreach (var tutor in list)
                {
                    if (tutor == id)
                    {
                        list.Remove(tutor);
                        break;
                    }
                }

                listOfFollwedTutors = ""; //Empty List

                foreach (var tutor in list)
                {
                    listOfFollwedTutors += tutor + "|";
                }

                return listOfFollwedTutors.Remove(listOfFollwedTutors.Length - 1);
            }

            return null;
        }

        public string AddToListOfFollowedStudents(string email, string listOfFollwedStudents) //for tutors
        {
            if (listOfFollwedStudents == null)
                listOfFollwedStudents = email;
            else
                listOfFollwedStudents += "|" + email;

            return listOfFollwedStudents;
        }

        public string RemoveFromListOfFollowedStudents(string email, string listOfFollwedStudents) //for tutors
        {
            if (email != null && listOfFollwedStudents != null)
            {
                if (!listOfFollwedStudents.Contains('|'))
                    return null;

                var list = listOfFollwedStudents.Split("|").ToList();

                foreach (var tutor in list)
                {
                    if (tutor == email)
                    {
                        list.Remove(tutor);
                        break;
                    }
                }

                listOfFollwedStudents = ""; //Empty List

                foreach (var tutor in list)
                {
                    listOfFollwedStudents += tutor + "|";
                }

                return listOfFollwedStudents.Remove(listOfFollwedStudents.Length - 1);
            }

            return null;
        }
    }
}
