using CompModels.Repositories.DTOs;

namespace CompModels.CRUD.Services.Validators
{
    public class BaseValidatorXYZ
    {
        public virtual IEnumerable<string> Validate(InputParamsValuesBase input)
        {
            if (input.X < 0)
                yield return $"{nameof(input.X)} не может быть меньше 0";

            if (input.Y < 0)
                yield return $"{nameof(input.Y)} не может быть меньше 0";

            if (input.Z < 0)
                yield return $"{nameof(input.Z)} не может быть меньше 0";

            if (input.X < 0)
                yield return $"{nameof(input.X)} не может быть меньше 0";
        }
    }
}
