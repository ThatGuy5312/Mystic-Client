using MysticClient.Menu;
using UnityEngine;

namespace MysticClient.Utils
{
    // if you take this i will hunt you
    public class GunLib : MonoBehaviour
    {
        public static bool leftHandGun;
        public static PrimitiveType gunShape = PrimitiveType.Cube;
        public static GameObject pointer = null;
        public static RaycastHit RaycastResult;
        public static Color enabledColor = Color.black;
        public static Color disabledColor = Color.black;
        private static LineRenderer LineRender = null;
        private static float pointerDelay = 15f;
        private static float bendAmount = .5f;
        public static int gunType;
        public static bool oldRayDirection;
        public static bool CreateGun(out VRRig rig)
        {
            var created = CreateGun(gunShape, out RaycastResult);
            rig = (RaycastResult.collider.GetComponentInParent<VRRig>() && created) ? RaycastResult.collider.GetComponentInParent<VRRig>() : null;
            if (rig && rig != RigUtils.MyOfflineRig && pointer.activeSelf)
                return CheckInputs();
            return false;
        }
        public static bool CreateGun(out RaycastHit hit, out VRRig rig)
        {
            var created = CreateGun(gunShape, out hit);
            rig = (RaycastResult.collider.GetComponentInParent<VRRig>() && created) ? RaycastResult.collider.GetComponentInParent<VRRig>() : null;
            if (rig && rig != RigUtils.MyOfflineRig && pointer.activeSelf)
                return CheckInputs();
            return false;
        }
        public static bool CreateGun() // this is so simple i love it
        {
            CreateGun(gunShape, out RaycastResult);
            return pointer.activeSelf && CheckInputs();
        }
        public static bool CreateGun(out RaycastHit hit)
        {
            CreateGun(gunShape, out hit);
            return pointer.activeSelf && CheckInputs();
        }
        public static bool CreateGun(bool customInput)
        {
            CreateGun(gunShape, out RaycastResult);
            return pointer.activeSelf && customInput;
        }
        public static bool CreateGun(bool customInput, out RaycastHit hit)
        {
            CreateGun(gunShape, out hit);
            return pointer.activeSelf && customInput;
        }
        public static bool CreateGun(bool customInput, out VRRig rig)
        {
            var created = CreateGun(gunShape, out RaycastResult);
            rig = (RaycastResult.collider.GetComponentInParent<VRRig>() && created) ? RaycastResult.collider.GetComponentInParent<VRRig>() : null;
            if (rig && rig != RigUtils.MyOfflineRig && pointer.activeSelf)
                return customInput;
            return false;
        }
        public static bool CreateGun(bool customInput, out RaycastHit hit, out VRRig rig)
        {
            var created = CreateGun(gunShape, out hit);
            rig = (RaycastResult.collider.GetComponentInParent<VRRig>() && created) ? RaycastResult.collider.GetComponentInParent<VRRig>() : null;
            if (rig && rig != RigUtils.MyOfflineRig && pointer.activeSelf)
                return customInput;
            return false;
        }
        public static void LockOnRig(VRRig rig) => pointer.transform.position = rig.head.headTransform.position;
        public static void LockOnObject(GameObject _object) => pointer.transform.position = _object.transform.position;
        private static bool CreateGun(PrimitiveType shape, out RaycastHit hit)
        {
            RenderGun(shape);
            if (Main.UserInput.GetMouseButton(1))
            {
                if (!Main.UserInput.GetMouseButton(1))
                {
                    pointer.SetActive(false);
                    LineRender.enabled = false;
                    Physics.Raycast(Main.mainCamera.ScreenPointToRay(Main.UserInput.mousePosition), out hit);
                    return false;
                }
                var hand = leftHandGun ? RigUtils.MyOnlineRig.leftHandTransform : RigUtils.MyOnlineRig.rightHandTransform;
                if (Physics.Raycast(Main.mainCamera.ScreenPointToRay(Main.UserInput.mousePosition), out hit, 1000f, GunMasks()))
                {
                    InitializeGun(hit, hand);
                    return true;
                }
            }
            else
            {
                var hand = leftHandGun ? RigUtils.MyOnlineRig.leftHandTransform : RigUtils.MyOnlineRig.rightHandTransform;
                var newRay = leftHandGun ? hand.forward : hand.forward;
                var oldRay = leftHandGun ? -hand.up : -hand.up;
                if (leftHandGun ? !Main.Controller.leftGrab : !Main.Controller.rightGrab)
                {
                    pointer.SetActive(false);
                    LineRender.enabled = false;
                    Physics.Raycast(hand.position, oldRayDirection ? oldRay : newRay, out hit, 1000f, GunMasks());
                    return false;
                }
                if (Physics.Raycast(hand.position, oldRayDirection ? oldRay : newRay, out hit, 1000f, GunMasks()))
                {
                    InitializeGun(hit, hand);
                    return true;
                }
            }
            return false;
        }
        private static void RenderGun(PrimitiveType shape)
        {
            if (pointer == null)
            {
                pointer = GameObject.CreatePrimitive(shape);
                pointer.GetComponent<Renderer>().material.shader = Main.TextShader;
                pointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                Destroy(pointer.GetComponent<Rigidbody>());
                Destroy(pointer.GetComponent<Collider>());
                pointer.SetActive(false);
                LineRender = pointer.AddComponent<LineRenderer>();
                LineRender.startWidth = 0.025f;
                LineRender.endWidth = 0.025f;
                LineRender.positionCount = 3;
                LineRender.useWorldSpace = true;
                LineRender.material.shader = Main.TextShader;
                LineRender.enabled = false;
            }
        }
        private static int pointCount = 100;
        private static void InitializeGun(RaycastHit hit, Transform hand)
        {
            pointer.transform.position = (gunType == 1) ? hit.point : Vector3.Lerp(pointer.transform.position, hit.point, Time.deltaTime * pointerDelay);
            if (gunType == 1)
            {
                LineRender.positionCount = 2;
                LineRender.SetPosition(0, hand.position);
                LineRender.SetPosition(1, pointer.transform.position);
            }
            else if (gunType == 0)
            {
                //var midPos = Vector3.Lerp(hand.position, pointer.transform.position, .5f) + Vector3.up * bendAmount;
                LineRender.positionCount = 3;
                LineRender.SetPosition(0, hand.position);
                LineRender.SetPosition(1, GetMiddle(hand.position + pointer.transform.position) + Vector3.up * bendAmount);
                LineRender.SetPosition(2, pointer.transform.position);
            }
            else if (gunType == 2)
            {
                var curvePoints = new Vector3[pointCount];
                for (int i = 0; i < pointCount; i++)
                {
                    var t = i / (float)(pointCount - 1);
                    curvePoints[i] = CalculatePoint(t, hand.position, GetMiddle(hand.position + hit.point), pointer.transform.position);
                }
                LineRender.positionCount = pointCount;
                LineRender.SetPositions(curvePoints);
            }
            else if (gunType == 3)
            {
                // so many
                var startPos = hand.position;
                var endPos = pointer.transform.position;
                var distance = Vector3.Distance(startPos, endPos);
                int pointCount = Mathf.CeilToInt(100 + distance * 10);
                var points = new Vector3[pointCount];
                var direction = (endPos - startPos).normalized;
                var right = Vector3.Cross(direction, Vector3.up).normalized;
                var up = Vector3.Cross(direction, right).normalized;
                var timeAngle = Time.time * 2;
                var revolutions = Mathf.Clamp(distance, 2, 15);
                var radius = .03f;

                for (int i = 0; i < pointCount; i++)
                {
                    var t = (float)i / (pointCount - 1);
                    var angle = t * revolutions * Mathf.PI * 2;
                    var currentAngle = timeAngle + angle;
                    var localOffset = CalculateOffset(currentAngle, direction, angle, right, up);
                    points[i] = startPos + direction * (t * distance) + localOffset * radius;
                }

                LineRender.positionCount = pointCount;
                LineRender.SetPositions(points);
            }
            pointer.SetActive(true);
            LineRender.enabled = true;
            pointer.transform.Rotate(0.8f, 0.8f, 0.8f);
            pointer.transform.localScale = Vector3.Lerp(
                new Vector3(0.2f, 0.2f, 0.2f),
                new Vector3(0.3f, 0.3f, 0.3f),
                Mathf.PingPong(Time.time, 1));
        }
        private static bool CheckInputs()
        {
            bool on = leftHandGun ? (Main.Controller.leftControllerIndexFloat.TriggerDown() || Main.UserInput.GetMouseButton(0)) :
                (Main.Controller.rightControllerIndexFloat.TriggerDown() ||
                Main.UserInput.GetMouseButton(0));
            SetGunColor(on ? enabledColor : disabledColor);
            return on;
        }
        private static void SetGunColor(Color color)
        {
            pointer.GetComponent<Renderer>().material.color = color;
            LineRender.startColor = color;
            LineRender.endColor = color;
            LineRender.material.color = color;
        }
        private static Vector3 CalculatePoint(float t, Vector3 start, Vector3 mid, Vector3 end) => Mathf.Pow(1 - t, 2) * start + 2 * (1 - t) * t * mid + Mathf.Pow(t, 2) * end;
        private static Vector3 GetMiddle(Vector3 vector) => new Vector3(vector.x / 2f, vector.y / 2f, vector.z / 2f);
        private static Vector3 CalculateOffset(float currentAngle, Vector3 direction, float angle, Vector3 right, Vector3 up) => Quaternion.AngleAxis(Mathf.Rad2Deg * currentAngle, direction) * (right * Mathf.Cos(angle) + up * Mathf.Sin(angle));
        public static int GunMasks() => RigUtils.MyPlayer.locomotionEnabledLayers + LayerMask.GetMask("Gorilla Tag Collider", "Gorilla Body Collider", "Rope Swing");

        /*public class Gun : MonoBehaviour // this was made really shitty but idc 
        {
            private const int DefaultGunCount = 100;
            public List<bool> leftHandGun = new List<bool>(DefaultGunCount);
            public List<GameObject> pointer = new List<GameObject>(DefaultGunCount);
            private List<LineRenderer> lineRender = new List<LineRenderer>(DefaultGunCount);
            public List<RaycastHit> raycasts = new List<RaycastHit>(DefaultGunCount);
            public void LockOnRig(VRRig rig, int gunID) => pointer[gunID].transform.position = rig.head.headTransform.position;
            public void LockOnObject(GameObject _object, int gunID) => pointer[gunID].transform.position = _object.transform.position;

            public Gun()
            {
                for (int i = 0; i < DefaultGunCount; i++)
                {
                    leftHandGun.Add(false);
                    pointer.Add(null);
                    lineRender.Add(null);
                }
            }

            public bool CreateGun(out RaycastHit hit, int gunID)
            {
                CreateGunInternal(out hit, gunID);
                return pointer[gunID].activeSelf && CheckInputs(gunID);
            }

            public bool CreateGun(bool customInput, out RaycastHit hit, int gunID)
            {
                CreateGunInternal(out hit, gunID);
                return pointer[gunID].activeSelf && customInput;
            }

            private bool CreateGunInternal(out RaycastHit hit, int gunID)
            {
                RenderGun(gunID);
                if (Main.UserInput.GetMouseButton(1))
                {
                    if (!Main.UserInput.GetMouseButton(1))
                    {
                        pointer[gunID].SetActive(false);
                        lineRender[gunID].enabled = false;
                        Physics.Raycast(Main.mainCamera.ScreenPointToRay(Main.UserInput.mousePosition), out hit);
                        return false;
                    }

                    var hand = leftHandGun[gunID] ? RigUtils.MyOnlineRig.leftHandTransform : RigUtils.MyOnlineRig.rightHandTransform;
                    if (Physics.Raycast(Main.mainCamera.ScreenPointToRay(Main.UserInput.mousePosition), out hit, 1000f, GunMasks()))
                    {
                        InitializeGun(hit, hand, gunID);
                        return true;
                    }
                }
                else
                {
                    var hand = leftHandGun[gunID] ? RigUtils.MyOnlineRig.leftHandTransform : RigUtils.MyOnlineRig.rightHandTransform;
                    var newRay = hand.forward;
                    var oldRay = -hand.up;

                    if (leftHandGun[gunID] ? !Main.Controller.leftGrab : !Main.Controller.rightGrab)
                    {
                        pointer[gunID].SetActive(false);
                        lineRender[gunID].enabled = false;
                        Physics.Raycast(hand.position, oldRay, out hit);
                        return false;
                    }
                    if (Physics.Raycast(hand.position, newRay, out hit))
                    {
                        InitializeGun(hit, hand, gunID);
                        return true;
                    }
                }
                return false;
            }

            private void RenderGun(int gunID)
            {
                if (pointer[gunID] == null)
                {
                    pointer[gunID] = GameObject.CreatePrimitive(gunShape);
                    pointer[gunID].GetComponent<Renderer>().material.shader = Main.TextShader;
                    pointer[gunID].transform.localScale = Vector3.one * 0.2f;
                    Destroy(pointer[gunID].GetComponent<Rigidbody>());
                    Destroy(pointer[gunID].GetComponent<Collider>());
                    pointer[gunID].SetActive(false);
                    lineRender[gunID] = pointer[gunID].AddComponent<LineRenderer>();
                    lineRender[gunID].startWidth = 0.025f;
                    lineRender[gunID].endWidth = 0.025f;
                    lineRender[gunID].positionCount = 3;
                    lineRender[gunID].useWorldSpace = true;
                    lineRender[gunID].material.shader = Main.TextShader;
                    lineRender[gunID].enabled = false;
                }
            }

            private void InitializeGun(RaycastHit hit, Transform hand, int gunID)
            {
                pointer[gunID].transform.position = (gunType == 1) ? hit.point : Vector3.Lerp(pointer[gunID].transform.position, hit.point, Time.deltaTime * pointerDelay);
                if (gunType == 1)
                {
                    lineRender[gunID].positionCount = 2;
                    lineRender[gunID].SetPosition(0, hand.position);
                    lineRender[gunID].SetPosition(1, pointer[gunID].transform.position);
                }
                else if (gunType == 0)
                {
                    lineRender[gunID].positionCount = 3;
                    lineRender[gunID].SetPosition(0, hand.position);
                    lineRender[gunID].SetPosition(1, GetMiddle(hand.position + pointer[gunID].transform.position) + Vector3.up * bendAmount);
                    lineRender[gunID].SetPosition(2, pointer[gunID].transform.position);
                }
                else if (gunType == 2)
                {
                    var curvePoints = new Vector3[pointCount];
                    for (int i = 0; i < pointCount; i++)
                    {
                        var t = i / (float)(pointCount - 1);
                        curvePoints[i] = CalculatePoint(t, hand.position, GetMiddle(hand.position + hit.point), pointer[gunID].transform.position);
                    }
                    lineRender[gunID].positionCount = pointCount;
                    lineRender[gunID].SetPositions(curvePoints);
                }
                else if (gunType == 3)
                {
                    // so many
                    var startPos = hand.position;
                    var endPos = pointer[gunID].transform.position;
                    var distance = Vector3.Distance(startPos, endPos);
                    int pointCount = Mathf.CeilToInt(100 + distance * 10);
                    var points = new Vector3[pointCount];
                    var direction = (endPos - startPos).normalized;
                    var right = Vector3.Cross(direction, Vector3.up).normalized;
                    var up = Vector3.Cross(direction, right).normalized;
                    var timeAngle = Time.time * 2;
                    var revolutions = Mathf.Clamp(distance, 2, 15);
                    var radius = .03f;

                    for (int i = 0; i < pointCount; i++)
                    {
                        var t = (float)i / (pointCount - 1);
                        var angle = t * revolutions * Mathf.PI * 2;
                        var currentAngle = timeAngle + angle;
                        var localOffset = CalculateOffset(currentAngle, direction, angle, right, up);
                        points[i] = startPos + direction * (t * distance) + localOffset * radius;
                    }

                    lineRender[gunID].positionCount = pointCount;
                    lineRender[gunID].SetPositions(points);
                }
                pointer[gunID].SetActive(true);
                lineRender[gunID].enabled = true;
                pointer[gunID].transform.Rotate(0.8f, 0.8f, 0.8f);
                pointer[gunID].transform.localScale = Vector3.Lerp(
                    new Vector3(0.2f, 0.2f, 0.2f),
                    new Vector3(0.3f, 0.3f, 0.3f),
                    Mathf.PingPong(Time.time, 1));
            }

            private bool CheckInputs(int gunID)
            {
                var on = leftHandGun[gunID] ? Main.Controller.leftControllerIndexFloat.TriggerDown() || Main.UserInput.GetMouseButton(0)
                                            : Main.Controller.rightControllerIndexFloat.TriggerDown() || Main.UserInput.GetMouseButton(0);
                SetGunColor(on ? Color.green : Color.red, gunID);
                return on;
            }

            private void SetGunColor(Color color, int gunID)
            {
                pointer[gunID].ChangeColor(color);
                lineRender[gunID].startColor = color;
                lineRender[gunID].endColor = color;
                lineRender[gunID].material.color = color;
            }
        }*/

        /*public class GunLibrary
        {
            public static GunData CreateGun(Action gunAction)
            {
                if (ControllerInputPoller.instance.rightGrab)
                {
                    var gunData = new GunData { sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere) };

                    UnityEngine.Object.Destroy(gunData.sphere.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(gunData.sphere.GetComponent<Collider>());

                    gunData.sphere.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    gunData.sphere.GetComponent<Renderer>().material.color = Color.blue; // color of gun

                    UnityEngine.Object.Destroy(gunData.sphere, Time.deltaTime);

                    if (Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, -GorillaTagger.Instance.rightHandTransform.up, out gunData.raycast))
                    {
                        gunData.sphere.transform.position = gunData.raycast.point;

                        gunData.line = gunData.sphere.AddComponent<LineRenderer>();
                        gunData.line.startWidth = .025f;
                        gunData.line.endWidth = .025f;
                        gunData.line.positionCount = 2;
                        gunData.line.useWorldSpace = true;
                        gunData.line.material.shader = Shader.Find("GUI/Text Shader");

                        gunData.line.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                        gunData.line.SetPosition(0, gunData.raycast.point);

                        if (ControllerInputPoller.instance.rightControllerIndexFloat > .5f)
                        {
                            gunData.rigHit = gunData.raycast.collider.GetComponentInParent<VRRig>()?? null;
                            gunAction();
                        }
                    }
                }
                return null;
            }

            public class GunData
            {
                public GameObject sphere;
                public LineRenderer line;
                public RaycastHit raycast;
                public VRRig rigHit;
            }
        }*/
    }
}