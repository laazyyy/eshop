using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using wangxu;

namespace wangxut { 
public class PizzaMenu
{
   
	
	
    private MenuItem[] items = new MenuItem[50];
	private PizzaMenu() {
			
		MenuItem item1 = new MenuItem();
		item1.setName("Chicken Parmesan");
		item1.setDescription("Grilled chicken, fresh tomatoes, feta and mozzarella cheese");
		item1.setPrice("10");
		item1.setIcon("/images/1.png");
		items[0] = item1;
		
		MenuItem item2 = new MenuItem();
		item2.setName("Spicy Italian");
		item2.setDescription("Pepperoni and a double portion of spicy Italian sausage");
		item2.setPrice("10");
		item2.setIcon("/images/2.png");
        items[1] = item2;
		
		MenuItem item3 = new MenuItem();
		item3.setName("Garden Fresh");
		item3.setDescription("Slices onions and green peppers, gourmet " +
				"mushrooms, black olives and ripe Roma tomatoes");
		item3.setPrice("10");
		item3.setIcon("/images/3.png");
        items[2] = item3;
		
		MenuItem item4 = new MenuItem();
		item4.setName("Tuscan Six Cheese");
		item4.setDescription("Six cheese blend of mozzarella, Parmesan, Romano, Asiago and Fontina");
		item4.setPrice("10");
		item4.setIcon("/images/4.png");
        items[3] = item4;
		
		MenuItem item5 = new MenuItem();
		item5.setName("Spinach Alfredo");
		item5.setDescription("Rich and creamy blend of spinach and garlic Parmesan with Alfredo sauce");
		item5.setPrice("10");
		item5.setIcon("/images/5.png");
        items[4] = item5;
		
		MenuItem item6 = new MenuItem();
		item6.setName("BBQ Chicken Bacon");
		item6.setDescription("Grilled white chicken, hickory-smoked bacon and fresh sliced onions in barbeque sauce");
		item6.setPrice("10");
		item6.setIcon("/images/6.png");
        items[5] = item6;
		
		MenuItem item7 = new MenuItem();
		item7.setName("Hawaiian BBQ Chicken");
		item7.setDescription("Grilled white chicken, hickory-smoked bacon, barbeque sauce topped with sweet pine-apple");
		item7.setPrice("34");
		item7.setIcon("/images/7.png");
        items[6] = item7;
		
		MenuItem item8 = new MenuItem();
		item8.setName("Grilled Chicken Club");
		item8.setDescription("Grilled white chicken, hickory-smoked bacon and fresh sliced onions topped with Roma tomatoes");
		item8.setPrice("440");
		item8.setIcon("/images/8.png");
        items[7] = item8;
		
		MenuItem item9 = new MenuItem();
		item9.setName("Double Bacon 6Cheese");
		item9.setDescription("Hickory-smoked bacon, Julienne cut Canadian bacon, Parmesan, " +
				"mozzarella, Romano, Asiago and and Fontina cheese");
		item9.setPrice("103");
		item9.setIcon("/images/9.png");
        items[8] = item9;
		
		MenuItem item10 = new MenuItem();
		item10.setName("Chilly Chicken Cordon Bleu");
		item10.setDescription("Spinash Alfredo sauce topped with grilled chicken, ham, onions and " +
				"mozzarella");
		item10.setPrice("103");
		item10.setIcon("/images/10.png");
        items[9] = item10;
	}
	
	public static PizzaMenu getInstance() {
		return new PizzaMenu();
	}
	
	public MenuItem[] getMenu() {
		return items;
	}
}

}