using UnityEngine;
using System.Collections;

public class SatelliteController : MonoBehaviour {

	public GameObject fixedStar;
	private int currentSatellite = 0;
	public GameObject[] satelliteObjects;

	private PositionOfSatellite currentPosition;

	// Use this for initialization
	void Start () {
		this.currentPosition = calculateSelfAngle ();
	}

	private GameObject getSatelliteObject() {
		if (this.satelliteObjects.Length == 0) {
			this.satelliteObjects = new GameObject[]{gameObject};
		}
		else if (this.satelliteObjects [0] == null) {
			this.satelliteObjects [0] = gameObject;
		}

		return this.satelliteObjects [currentSatellite];
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("f")) {
//Debug.Log("Vertical: " + this.currentPosition.getVerticalAngle());
			setHorizontalAngle (getSatelliteObject(), this.currentPosition.getHorizontalAngle());
		}

		if (Input.GetKeyDown ("g")) {
// Debug.Log("Vertical: " + this.currentPosition.getVerticalAngle());
			setVerticalAngle (getSatelliteObject(), this.currentPosition.getVerticalAngle ());
		}

		int sign = 1;

		if (Input.GetKey (KeyCode.RightShift) || Input.GetKey (KeyCode.LeftShift)) {
			sign = -1;
		}

		bool rotation = false;
		if (Input.GetKey (KeyCode.Backslash)) {
			rotation = true;
		}

		if (Input.GetKeyDown ("s")) {
			seekSatellite(sign == 1);
		}

		if (Input.GetKey ("x")) {
			if (rotation) {
				rotate (Vector3.right * sign);
			}
			else {
				translate(Vector3.right * sign * 0.1f);
			}
		}

		if (Input.GetKey ("y")) {
			if (rotation) {
				rotate (Vector3.up * sign);
			}
			else {
				translate(Vector3.up * sign * 0.1f);
			}
		}

		if (Input.GetKey ("z")) {
			if (rotation) {
				rotate (Vector3.forward * sign);
			}
			else {
				translate(Vector3.forward * sign * 0.1f);
			}
		}

		if (Input.GetKey ("c")) {
			translateCircling(1.0f * sign);
			setHorizontalAngle (getSatelliteObject(), this.currentPosition.getHorizontalAngle());
		}

	}

	private void seekSatellite(bool sign) {
		while (this.satelliteObjects [seekToNextIndex(sign)] == null) {	}
		this.currentPosition = calculateSelfAngle ();
	}

	private int seekToNextIndex(bool sign) {
		if (sign) {
			this.currentSatellite++;
			if (this.currentSatellite >= this.satelliteObjects.Length) {
				this.currentSatellite = 0;
			}
		} else {
			this.currentSatellite--;
			if (this.currentSatellite < 0) {
				this.currentSatellite = this.satelliteObjects.Length - 1;
			}
		}
		return this.currentSatellite;
	}

	private void translateCircling(float angle) {
		Vector3 previousPosition = getSatelliteObject().transform.position;
		Vector3 distanceFromPreviousPosition = new Vector3 ();
		distanceFromPreviousPosition.x = fixedStar.transform.position.x - previousPosition.x;
		distanceFromPreviousPosition.z = fixedStar.transform.position.z - previousPosition.z;

		this.currentPosition.increaseHorizontalAngle (angle);

		float currentAngle = this.currentPosition.getHorizontalAngle ();


		Vector3 distanceToTheStar = new Vector3 ();

		distanceToTheStar.x = 
			this.currentPosition.getDistance() * 
				Mathf.Cos (currentAngle * Mathf.Deg2Rad);

		distanceToTheStar.z = 
				this.currentPosition.getDistance() * 
				Mathf.Sin (currentAngle * Mathf.Deg2Rad);

		float tmpY = getSatelliteObject().transform.localPosition.y;
		Vector3 tmpPos = fixedStar.transform.position - distanceToTheStar;
		getSatelliteObject().transform.localPosition = new Vector3 (tmpPos.x, tmpY, tmpPos.z);
	}

	private void translate(Vector3 distance) {
		getSatelliteObject().transform.Translate(distance);
		this.currentPosition = calculateSelfAngle ();
//Debug.Log ("Angle: " + this.currentPosition.getAngle ());

	}

	private void rotate(Vector3 angles) {
		getSatelliteObject().transform.Rotate(angles);
	}

	private PositionOfSatellite calculateSelfAngle() {
		return calculateAngleBetween (getSatelliteObject(), fixedStar);
	}

	private static PositionOfSatellite calculateAngleBetween(GameObject target, GameObject fixedObject) {
		Vector3 distance = fixedObject.transform.position - target.transform.position;

		return new PositionOfSatellite (
			calculateAngleBySides(new Vector2(distance.x, distance.z)), 
			calculateAngleBySides(new Vector2(-distance.y, 
				Mathf.Sqrt(Mathf.Pow (distance.x, 2) + Mathf.Pow (distance.z, 2))
		                                  )),
			Mathf.Sqrt(Mathf.Pow(distance.z, 2) + Mathf.Pow(distance.x, 2)));
	}

	private static float calculateAngleBySides(Vector2 distance) {
		float result = Mathf.Atan (distance.y / distance.x) * Mathf.Rad2Deg;
		if (distance.x < 0.0f) {
			result += 180.0f;
		}
		return result;
	}

	private static void setHorizontalAngle(GameObject target, float angle) {
		target.transform.Rotate (new Vector3 (0.0f, (450.0f - angle) % 360.0f - target.transform.localEulerAngles.y, 0.0f));
	}

	private static void setVerticalAngle(GameObject target, float angle) {
		target.transform.Rotate (new Vector3 ((450.0f - angle) % 360.0f - target.transform.localEulerAngles.x, 0.0f, 0.0f));
	}

	//	private void focusOnTheStarOnRelativeX () {
	//	}
}
