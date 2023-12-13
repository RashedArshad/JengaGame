using UnityEngine;
public class CameraRotator : MonoBehaviour
{
    #region Private Members 
    [Range(0.1f, 5f)]
    [Tooltip("How sensitive the mouse drag to camera rotation")]
    [SerializeField] private float _mouseRotateSpeed = 0.8f;
    [Range(0.01f, 100)]
    [Tooltip("How sensitive the touch drag to camera rotation")]
    [SerializeField] private float _touchRotateSpeed = 17.5f;
    [Tooltip("Smaller positive value means smoother rotation, 1 means no smooth apply")]
    [SerializeField] private float _slerpValue = 0.25f;
    [Tooltip("How do you like to rotate the camera")]
    [SerializeField] private RotateMethod _rotateMethod = RotateMethod.Mouse;
    private enum RotateMethod { Mouse, Touch };

    [SerializeField] private Transform _target; // The stacks the camera will be focusing on them 
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _changeFocusSpeed = 2;

    private Vector2 _swipeDirection; //swipe delta vector2
    private Quaternion _cameraRot; // store the quaternion after the slerp operation
    private Touch _touch;
    private float _distanceBetweenCameraAndTarget;

    private float _minXRotAngle = -80; //min angle around x axis
    private float _maxXRotAngle = 80; // max angle around x axis

    //Mouse rotation related
    private float _rotX; // around x
    private float _rotY; // around y
    private bool _focusChanged;

    #endregion

    #region Unity CallBacks 
    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
    }
    private void Start()
    {
        _distanceBetweenCameraAndTarget = Vector3.Distance(_mainCamera.transform.position, _target.position);
    }

    private void Update()
    {
        if (_rotateMethod == RotateMethod.Mouse)
        {
            if (Input.GetMouseButton(0))
            {
                _rotX += -Input.GetAxis("Mouse Y") * _mouseRotateSpeed; // around X
                _rotY += Input.GetAxis("Mouse X") * _mouseRotateSpeed;
            }

            if (_rotX < _minXRotAngle)
            {
                _rotX = _minXRotAngle;
            }
            else if (_rotX > _maxXRotAngle)
            {
                _rotX = _maxXRotAngle;
            }
        }
        else if (_rotateMethod == RotateMethod.Touch)
        {
            if (Input.touchCount > 0)
            {
                _touch = Input.GetTouch(0);
                if (_touch.phase == TouchPhase.Began)
                {
                    //Debug.Log("Touch Began");

                }
                else if (_touch.phase == TouchPhase.Moved)
                {
                    _swipeDirection += _touch.deltaPosition * Time.deltaTime * _touchRotateSpeed;
                }
                else if (_touch.phase == TouchPhase.Ended)
                {
                    //Debug.Log("Touch Ended");
                }
            }

            if (_swipeDirection.y < _minXRotAngle)
            {
                _swipeDirection.y = _minXRotAngle;
            }
            else if (_swipeDirection.y > _maxXRotAngle)
            {
                _swipeDirection.y = _maxXRotAngle;
            }
        }

    }

    private void LateUpdate()
    {

        Vector3 dir = new Vector3(0, 0, -_distanceBetweenCameraAndTarget); //assign value to the distance between the maincamera and the target

        Quaternion newQ; // value equal to the delta change of our mouse or touch position
        if (_rotateMethod == RotateMethod.Mouse)
        {
            newQ = Quaternion.Euler(_rotX, _rotY, 0); //We are setting the rotation around X, Y, Z axis respectively
        }
        else
        {
            newQ = Quaternion.Euler(_swipeDirection.y, -_swipeDirection.x, 0);
        }
        _cameraRot = Quaternion.Slerp(_cameraRot, newQ, _slerpValue);  //let cameraRot value gradually reach newQ which corresponds to our touch
        if (_focusChanged)
        {
            _mainCamera.transform.position = Vector3.Lerp(transform.position, _target.position + _cameraRot * dir,Time.deltaTime* _changeFocusSpeed);
            //mainCamera.transform.LookAt(target.position);
            Invoke(nameof(Reached), 0.5f);
        }
        else
        {
            _mainCamera.transform.position = _target.position + _cameraRot * dir;
            _mainCamera.transform.LookAt(_target.position);
        }
    }

    #endregion

    #region Private Methods
    private void Reached()
    {
        _focusChanged = false;
    }

    #endregion

    #region public Methods
    // called on the toggles for each grade
    public void ChangeFocusPoint( Transform newTarget)
    {
        CancelInvoke(nameof(Reached));
        _focusChanged = true;
        _target = newTarget;
    }
    public void SetCurrentStackGrade(int grade)
    {
        GameManager.Instance.CurrentStackNum = grade;
    }

    #endregion
    public void SetCamPos()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
        _mainCamera.transform.position = new Vector3(0, 0, -_distanceBetweenCameraAndTarget);
    }
}