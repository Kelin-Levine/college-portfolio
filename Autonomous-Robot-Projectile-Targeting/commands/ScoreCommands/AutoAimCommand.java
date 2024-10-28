package frc.robot.commands.ScoreCommands;

import edu.wpi.first.wpilibj2.command.Command;
import frc.robot.subsystems.BlinkinSubsystem;
import frc.robot.subsystems.EndEffectorSubsystem;
import frc.robot.subsystems.LimelightSubsystem;

public class AutoAimCommand extends Command {

   
    private final LimelightSubsystem limelightSubsystem;
    private boolean atTarget;
    
    public AutoAimCommand(LimelightSubsystem limelightSubsystem){
        this.limelightSubsystem = limelightSubsystem;
        this.atTarget = false;
        //addRequirements(limelightSubsystem);
    }


    @Override 
    public void execute(){
        Double target = limelightSubsystem.getAutoAimEncoderTarget();
        if (target != null) EndEffectorSubsystem.lift(target);
        atTarget = EndEffectorSubsystem.isLiftAtTarget();
    }

    @Override
    public void end(boolean interrupted){
        atTarget = false;
        BlinkinSubsystem.green();
    }

    public boolean isAtTarget() {
        return atTarget;
    }

}

