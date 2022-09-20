﻿using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;
using TopLearn.Core.Convertors;
using TopLearn.Core.Generator;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Core.Services.Services
{
    public class UserService : IUserService
    {

        private TopLearnContext _context;

        public UserService(TopLearnContext context)
        {
            _context = context;
        }

        public bool IsExistEmail(string email)
        {
            return _context.users.Any(p => p.Email == email);
        }

        public int AddUser(User user)
        {

            _context.users.Add(user);
            _context.SaveChanges();
            return user.UserId;

        }

        public User LoginUser(string Email, string Password)
        {
            var HashedPassword = PasswordHelper.EncodePasswordMd5(Password);
            var FixedEmail = Email.FixEmail();
            return _context.users.SingleOrDefault(user => user.Password == HashedPassword && user.Email == FixedEmail);
        }

        public User ActiveAccount(string ActiveCode)
        {
            var user = _context.users.SingleOrDefault(u => u.ActiveCode == ActiveCode);
            if (user == null || user.IsActive)
                return null;

            user.IsActive = true;
            user.ActiveCode = TextGenerator.GenerateUniqCode();
            _context.SaveChanges();

            return user;

        }

        public User GetUserByEmail(string Email)
        {
            return _context.users.SingleOrDefault(p => p.Email == Email);
        }

        public User GetUserByActiveCode(string ActiveCode)
        {
            return _context.users.SingleOrDefault(u => u.ActiveCode == ActiveCode);
        }

        public void UpdateUser(User user)
        {
            user.ActiveCode = TextGenerator.GenerateUniqCode();
            _context.users.Update(user);
            _context.SaveChanges();
        }
    }
}
