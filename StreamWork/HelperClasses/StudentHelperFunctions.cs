
using System.Linq;

namespace StreamWork.HelperClasses
{
    public class StudentHelperFunctions ////For functions involved with student code only
    {
       public string AddToListOfFollowedTutors(string id, string listOfFollwedTutors)
       {
            if(listOfFollwedTutors == null)
                listOfFollwedTutors = id;
            else
                listOfFollwedTutors += "|" + id;

            return listOfFollwedTutors;
       }

        public string RemoveFromListOfFollowedTutors(string id, string listOfFollwedTutors)
        {
           if(id != null && listOfFollwedTutors != null)
           {
                if (!listOfFollwedTutors.Contains('|'))
                    return null;

                var list = listOfFollwedTutors.Split("|").ToList();

                foreach(var tutor in list)
                {
                    if(tutor == id)
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
    }
}
