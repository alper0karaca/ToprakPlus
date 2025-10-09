using GenericRepository;
using ToprakPlusServer.Domain.Users;

namespace ToprakPlusServer.WebAPI;

public static class ExtensionMethods
{
    public static async Task CreateFirstUser(this WebApplication app)
    {
        using var scoped = app.Services.CreateScope();
        var userRepository = scoped.ServiceProvider.GetRequiredService<IUserRepository>();
        var unitOfWork = scoped.ServiceProvider.GetRequiredService<IUnitOfWork>();

        if (!(await userRepository.AnyAsync(x => x.UserName.Value == "admin")))
        {
            FirstName firstName = new FirstName("Alper");
            LastName lastName = new LastName("Karaca");
            Email email = new Email("alper0karaca@gmail.com");
            UserName userName = new UserName("alper0karaca");
            Password password = new Password("2204");

            User firstUser = new User(
                firstName, 
                lastName, 
                email, 
                userName, 
                password);
            
            userRepository.Add(firstUser);
            await unitOfWork.SaveChangesAsync();
        }
    } 
}