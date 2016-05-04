difference() {
    hoogte = 146;
    breedte = 176;
    diepte = 46;
    laserdiodeHoogte = 15;
    photodiodeHoogte = 8;
    
    // Omtrek
    cube([hoogte, breedte, diepte]);
    
    // Uitsnede rechtsonder
    translate([45, 44, 0]) {
        cube([105, breedte - 20, diepte]);
    }
    
    // Uitsnede boven
    cube([32, breedte - 25, diepte]);
    
    // Gat links
    translate([64, 8, 0]) {
        cube([hoogte - 8 - 64, 29, diepte]);
    }
    
    // Laserdiode uitsnede
    translate([0, breedte - laserdiodeHoogte, diepte / 2]) {
        rotate([-90, 0, 0]) {
            cylinder(laserdiodeHoogte, r = 7);
        }
    }
    
    // Photodiode uitsnede
    translate([45, breedte - photodiodeHoogte, diepte / 2]) {
        rotate([-90, 0, 0]) {
            cylinder(photodiodeHoogte, r = 5);
        }
    }
}
