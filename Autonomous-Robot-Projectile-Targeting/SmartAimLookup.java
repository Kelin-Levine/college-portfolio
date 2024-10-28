// Copyright (c) FIRST and other WPILib contributors.
// Open Source Software; you can modify and/or share it under the terms of
// the WPILib BSD license file in the root directory of this project.

package frc.robot;

import java.util.HashMap;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;

/** Populates and manages a lookup table for the calculation of the angle of the shooter in smart aiming, precise to the nearest inch.
 * The full equation is too complex to write out here. See this desmos graph:
 * https://www.desmos.com/calculator/x34zsq9ch5
*/
public class SmartAimLookup {

    private static final int TABLE_START = 30;
    private static final int TABLE_END = 510;
    private static final int TABLE_FACTOR = 3; // How many calculations to have per inch (i.e. = 2 means two calculations per inch, or one every 0.5 inches)

    private static final int APPROXIMATION_PRECISION = 1000;

    private static final boolean DUAL_CORE_SPEEDUP = true;

    private static HashMap<Integer, Double> table;

    public static void populateTable() {
        int start = TABLE_START * TABLE_FACTOR;
        int end = TABLE_END * TABLE_FACTOR;
        
        boolean dualCoreSuccess = DUAL_CORE_SPEEDUP;
        if (DUAL_CORE_SPEEDUP) {
            int half_upper = (int) Math.ceil((end - start) / 2.0);

            ExecutorService executor = Executors.newFixedThreadPool(2);
            try {
                Future<HashMap<Integer, Double>> f1 = executor.submit(() -> populateSegment(start, start + half_upper));
                Future<HashMap<Integer, Double>> f2 = executor.submit(() -> populateSegment(start + half_upper, end + 1));
                table = new HashMap<>(end - start + 2, 1);
                table.putAll(f1.get());
                table.putAll(f2.get());
            } catch (ExecutionException | InterruptedException ex) {
                dualCoreSuccess = false;
            }
            executor.shutdown();
        }

        if (!dualCoreSuccess) {
            table = populateSegment(start, end + 1);
        }
    }

    private static HashMap<Integer, Double> populateSegment(int start, int end) {
        HashMap<Integer, Double> segment = new HashMap<>(end - start + 1, 1);

        for (int i = start; i < end; i++) {
            Double approximation = approximateAngle((double) i / TABLE_FACTOR);

            if (approximation != null) segment.put(i, approximation);
        }

        return segment;
    }

    private static Double approximateAngle(double dx) {
        double approx = Math.PI / 8; // start at 22.5 degrees so that it always finds the lower possible angle first
        double step = Math.PI / 8; // because of math, the higher possible angle will never be lower than 45 degrees, I think
        Integer stepDirection = 0;
        Integer lastStepDirection = 0;
        for (int i = 0; i < APPROXIMATION_PRECISION; i++) {
            stepDirection = approximationStepDirection(approx, dx);
            if (stepDirection == null) return null;
            if (stepDirection == 0) return approx;

            if (i != 0 && stepDirection != lastStepDirection) step /= 2;
            approx += step * stepDirection;

            lastStepDirection = stepDirection;
        }
        if (approx < 0 || approx > Math.PI / 2) return null;
        return approx;
    }

    private static Integer approximationStepDirection(double approx, double dx) {
        //double result = getResult(approx, dh);
        double result = getResultWithDrag(approx, dx);
        if (Double.isNaN(result)) return null;

        if (result > 0) {
            return -1;
        } else if (result < 0) {
            return 1;
        } else {
            return 0;
        }
    }

    private static double getResult(double approx, double dx) {
        // This is the fun part.
        //double dx = Math.cos(Math.asin((SmartAimConstants.th - SmartAimConstants.camh) / dh)) * dh;
        double part1 = dx + SmartAimConstants.spo - SmartAimConstants.rox - Math.cos(approx) * SmartAimConstants.shox + Math.sin(approx) * SmartAimConstants.shoy;
        return part1 * Math.tan(approx)
                - ((SmartAimConstants.g * Math.pow(part1, 2)) / (2 * Math.pow(SmartAimConstants.nvi, 2) * Math.pow(Math.cos(approx), 2)))
                + (SmartAimConstants.roy + Math.sin(approx) * SmartAimConstants.shox + Math.cos(approx) * SmartAimConstants.shoy - SmartAimConstants.th - SmartAimConstants.sh);
        // Did you have fun?
    }

    private static double getResultWithDrag(double approx, double dx) {

        boolean useNewDrag = false;

        // This is the really fun part.
        double Av = Math.sin(approx) * SmartAimConstants.Ah + Math.cos(approx) * SmartAimConstants.Av;
        double Ah = Math.sin(approx) * SmartAimConstants.Av + Math.cos(approx) * SmartAimConstants.Ah;

        double Cv = SmartAimConstants.Cv;
        double Ch = SmartAimConstants.Ch;
        double nvi = SmartAimConstants.nvi;

        if (useNewDrag) {
            Cv = Math.sin(approx) * SmartAimConstants.ChNew + Math.cos(approx) * SmartAimConstants.CvNew;
            Ch = Math.sin(approx) * SmartAimConstants.CvNew + Math.cos(approx) * SmartAimConstants.ChNew;
            nvi = SmartAimConstants.nviNew;
        }

        double kv = 0.5 * SmartAimConstants.p * Av * Cv;
        double kh = 0.5 * SmartAimConstants.p * Ah * Ch;

        double part4 = Math.atan(nvi * Math.sin(approx) * Math.sqrt(kv / (SmartAimConstants.m * SmartAimConstants.g)));
        double part6 = Math.sqrt(SmartAimConstants.m / (SmartAimConstants.g * kv));
        double b1 = part6
                * part4;
        double part5 = (kv * Math.pow(nvi, 2)) / (SmartAimConstants.m * SmartAimConstants.g);
        /*double b2 = part6
                * ((1/Math.sqrt(1+Math.pow(Math.sqrt(1+Math.pow(Math.sin(approx),2)* part5),2)))
                + part4);*/

        //double dx = Math.cos(Math.asin((SmartAimConstants.th - SmartAimConstants.camh) / dh)) * dh;

        double x = (dx + SmartAimConstants.spo) - (SmartAimConstants.rox + Math.cos(approx) * SmartAimConstants.shox - Math.sin(approx) * SmartAimConstants.shoy);
        double y = (SmartAimConstants.th + SmartAimConstants.sh) - (SmartAimConstants.roy + Math.sin(approx) * SmartAimConstants.shox + Math.cos(approx) * SmartAimConstants.shoy);

        double t = (SmartAimConstants.m / (kh * nvi * Math.cos(approx))) * (Math.pow(Math.E, (kh * x) / SmartAimConstants.m) - 1);

        double result;
        double part1 = Math.sqrt(SmartAimConstants.g * kv / SmartAimConstants.m);
        double part2 = (SmartAimConstants.m / (2 * kv)) * Math.log(1 + part5 * Math.pow(Math.sin(approx), 2));
        double part3 = t * part1 - part4;
        if (t <= b1) {
            result = ((SmartAimConstants.m / kv) * Math.log(Math.cos(part3))
                    + part2)
                    - y;
        } else {
            result = (-(SmartAimConstants.m / kv) * Math.log(Math.cosh(part3))
                    + part2)
                    - y;
        }

        return result;
        // I blacked out for a few minutes and this strange math appeared on my screen. Should I see a doctor?
    }

    public static Double getAngle(double dx) {
        return table.get((int) Math.round(dx * TABLE_FACTOR));
    }

    public static double tyToDx(double ty) {
        // how many degrees back is your limelight rotated from perfectly vertical?

     // distance from the center of the Limelight lens to the floor
 
     // distance from the target to the floor
 
     double angleToGoalDegrees = SmartAimConstants.ca + ty;
     double angleToGoalRadians = Math.toRadians(angleToGoalDegrees);
 
     //calculate distance
     return (SmartAimConstants.th - SmartAimConstants.camh) / Math.tan(angleToGoalRadians);
    }
}
