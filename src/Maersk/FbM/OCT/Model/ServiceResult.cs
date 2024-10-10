using System.Net;

namespace Maersk.FbM.OCT.Model;

/// <summary>
/// Encapsulates the response type T into a payload that also replies with the http status value to return and any related
/// errors or messages to supply to the caller.
/// </summary>
/// <typeparam name="T">The domain model used for providing a response to the caller.</typeparam>
public class ServiceResult<T>
{
    private HttpStatusCode StatusCodeValue { get; set; } = HttpStatusCode.OK;
    private T ModelValue { get; set; }
    private List<Errors> ErrorsValue { get; set; }

    public ServiceResult(T Model) {
        this.ModelValue = Model;
    }

    public ServiceResult(HttpStatusCode StatusCode, T Model) {
        this.StatusCodeValue = StatusCode;
        this.ModelValue = Model;
    }

    public ServiceResult(HttpStatusCode StatusCode, List<Errors> Errors) {
        this.StatusCodeValue = StatusCode;
        this.ErrorsValue = Errors;
    }

    public ServiceResult(HttpStatusCode StatusCode, Errors Error) {
        this.StatusCodeValue = StatusCode;
        this.ErrorsValue = new List<Errors> { Error };
    }

    public ServiceResult(HttpStatusCode StatusCode, T Model, List<Errors> Errors) {
        this.StatusCodeValue = StatusCode;
        this.ModelValue = Model;
        this.ErrorsValue = Errors;
    }

    public ServiceResult(HttpStatusCode StatusCode, T Model, Errors Error) {
        this.StatusCodeValue = StatusCode;
        this.ModelValue = Model;
        this.ErrorsValue = new List<Errors> { Error };
    }

    public HttpStatusCode StatusCode => StatusCodeValue;

    public T Model => ModelValue;
    
    public List<Errors> Errors => ErrorsValue;

    public bool IsSuccessStatusCode() {
        return (int)this.StatusCode >= 200 && (int)this.StatusCode <= 299;
    }

    public ServiceResult<T1> ToError<T1>(T1 t1 = default(T1)) {
        return new ServiceResult<T1>(this.StatusCode, t1, this.Errors);
    }
}