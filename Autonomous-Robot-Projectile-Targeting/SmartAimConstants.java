// Copyright (c) FIRST and other WPILib contributors.
// Open Source Software; you can modify and/or share it under the terms of
// the WPILib BSD license file in the root directory of this project.

package frc.robot;

/** Add your docs here. */
public final class SmartAimConstants {
    /* Physical configuration:
    ch is the height of the camera from the ground (inches);
    ca is the angle of the camera back from vertical (degrees);
    th is the height of the apriltag from the ground (56.375 inches on a standard field);
    sh is the vertical distance between the center of the apriltag and the center of the speaker opening (24.0625 inches on a standard field);
    spo is the horizontal offset of the center of the speaker opening from the apriltag (-9 inches on a standard field);
    rox and roy are the x and y offsets of the center of rotation from the camera/limelight respectively;
    shox and shoy are the x and y offsets of the shooter from the center of rotation respectively (at angle 0);
    nvi is the initial velocity of the note (tune this empirically);
    g is gravity in inches/second^2 (always 386.1 unless you need to run the robot on the moon or something) */
    public static final double camh = 19.5;
    public static final double ca = 18.0;
    public static final double th = 57; // real field: 57.0; practice field: 54.25; standard: 56.375;
    public static final double sh = 21.5; // real field 23.0; practice field: 23.0; standard: 24.0625;
    public static final double spo = -9.0;
    public static final double rox = 1.785;
    public static final double roy = -6.25;
    public static final double shox = -6.633;
    public static final double shoy = 21.5;
    public static final double nvi = 790; // with old drag values, the best value is 790
    public static final double nviNew = 790; // with new drag values
    public static final double g = 386.1;

    /* Drag constants:
    m is the mass of the note (kg)
    p is the local air pressure in kg/in^3 (1 atm = 0.000020074 kg/in^3)
    Av is the vertical surface area of the note (such that the note is flat against a wall)
    Ah is the horizontal surface area of the note (such that the note is flat against a wall)
    Cv is the vertical drag coefficient of the note
    Ch is the horizontal drag coefficient of the note */
    public static final double m = 0.2353;
    public static final double p = 0.000020074;
    public static final double Av = 75.4;
    public static final double Ah = 28;
    public static final double Cv = 0.02;
    public static final double Ch = 0.15;
    public static final double CvNew = 0.4;
    public static final double ChNew = 0.5;
}
