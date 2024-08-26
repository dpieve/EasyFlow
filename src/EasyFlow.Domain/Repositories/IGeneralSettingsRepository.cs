//using EasyFlow.Domain.Entities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static System.Runtime.InteropServices.JavaScript.JSType;

//namespace EasyFlow.Domain.Repositories;
//public interface IGeneralSettingsRepository
//{
//    public Result<GeneralSettings, Error> Get();

//    public Task<Result<int, Error>> CreateAsync(GeneralSettings settings);

//    public Task<Result<int, Error>> DeleteAsync(GeneralSettings settings);

//    public Task<Result<int, Error>> UpdateAsync(GeneralSettings settings);

//    public Task<Result<bool, Error>> UpdateSelectedTagAsync(Tag tag);

//    public Result<Tag, Error> GetSelectedTag();
//    public void UpdateSelectedTheme(Theme theme);
//    public void UpdateSelectedColorTheme(ColorTheme colorTheme);

//    public bool IsFocusDescriptionEnabled();

//    public Task<Result<bool, Error>> UpdateSelectedLanguage(SupportedLanguage selectedLanguage);
//    public Result<SupportedLanguage, Error> GetSelectedLanguage();
//}
