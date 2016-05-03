difference() {
    hoogte = 46;
    
    // Omtrek
    cube([145, 175, 46]);
    
    // Uitsnede rechtsonder
    translate([45, 45, 0]) {
        cube([105, 155, hoogte]);
    }
    
    // Gat boven
    translate([0, 0, 0]) {
        cube([32, 150, hoogte]);
    }
    
    // Gat links
    translate([65, 8, 0]) {
        cube([73, 28, hoogte]);
    }
    
    // Laserdiode
    translate([0, 160, hoogte / 2]) {
        rotate(-90, [1, 0, 0]) {
            cylinder(15, r = 7);
        }
    }
    
    // Photodiode
    translate([45, 167, hoogte / 2]) {
        rotate(-90, [1, 0, 0]) {
            cylinder(8, r = 5);
        }
    }
}
    