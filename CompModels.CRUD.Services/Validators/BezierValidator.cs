using CompModels.Repositories.DTOs;
using CompModels.Repositories.DTOs.In;
using System.Text.Json;

namespace CompModels.CRUD.Services.Validators
{
    public class BezierValidator : BaseValidatorXYZ
    {
        public override IEnumerable<string> Validate(InputParamsValuesBase input)
        {
            foreach (var error in base.Validate(input))
                yield return error;

            var bezierInputParamsValues = input as BezierInputParamsValues;
            if (bezierInputParamsValues == null)
                throw new InvalidCastException($"{JsonSerializer.Serialize(input)} не является {nameof(bezierInputParamsValues)}");

            if (bezierInputParamsValues.FibreDiameter < 0)
                yield return $"{nameof(bezierInputParamsValues.FibreDiameter)} не может быть меньше 0";

            if (bezierInputParamsValues.DesiredPorosity < 0)
                yield return $"{nameof(bezierInputParamsValues.DesiredPorosity)} не может быть меньше 0";
        }
    }
}
