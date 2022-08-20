using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTests;

public class ProfileImage
{
    public static ValueTask<ProfileImage> Load(string FileName) => ValueTask.FromException<ProfileImage>(new NotImplementedException());
}
