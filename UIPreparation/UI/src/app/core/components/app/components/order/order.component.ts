import { Component, AfterViewInit, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AlertifyService } from 'app/core/services/alertify.service';
import { LookUpService } from 'app/core/services/lookUp.service';
import { AuthService } from 'app/core/components/admin/login/services/auth.service';
import { environment } from 'environments/environment';
import { Order } from './models/order';
import { Customer } from '../customer/models/customer';
import { CustomerService } from '../customer/services/customer.service';
import { OrderService } from './services/order.service';
import { OrderDto } from './models/orderDto';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { Product } from '../product/models/Product';
import { ProductService } from '../product/services/Product.service';
import { Store } from '../store/models/store';
import { StoreService } from '../store/services/store.service';
import { StoreDto } from '../store/models/storeDto';

declare var jQuery: any;

@Component({
	selector: 'app-order',
	templateUrl: './order.component.html',
	styleUrls: ['./order.component.scss']
})
export class OrderComponent implements AfterViewInit, OnInit {

	dataSource: MatTableDataSource<any>;
	@ViewChild(MatPaginator) paginator: MatPaginator;
	@ViewChild(MatSort) sort: MatSort;
	displayedColumns: string[] = [
		'customerName',
		'productName',
		'stock',
		'update',
		'delete'
	];

	orderList: Order[];
	orderDtoList: OrderDto[];
	order: Order = new Order();
	storeDtoList: StoreDto[];
	customerList: Customer[];

	orderAddForm: FormGroup;

	customerControl = new FormControl();
	storeDtoControl = new FormControl();
	filteredOptionsWithStoreDto: Observable<StoreDto[]>;
	filteredOptionsWithCustomer: Observable<Customer[]>;




	orderId: number;

	constructor(private orderService: OrderService, private lookupService: LookUpService, private alertifyService: AlertifyService, private formBuilder: FormBuilder, private authService: AuthService, private productService: ProductService, private storeService: StoreService, private customerService: CustomerService) { }

	ngAfterViewInit(): void {
		this.getOrderDtoList();
	}

	ngOnInit() {
		this.getCustomersLookUp();
		this.getStoresLookUp();
		this.createOrderAddForm();

	}

	getStoresLookUp() {
		this.storeService.getStoreDtoList().subscribe(data => {
			this.storeDtoList = data;			

			this.filteredOptionsWithStoreDto = this.storeDtoControl.valueChanges.pipe(
				startWith(''),
				map(value => {
				  const name = typeof value === 'string' ? value : value?.name;
				  return name ? this._filterStoreDto(name as string) : this.storeDtoList.slice();
				}))
		});
	}

	displayFnStoreDto(data: StoreDto): string {
		return data && data.productName ? data.productName : '';
	  }
	
	  private _filterStoreDto(name: string): StoreDto[] {
		const filterValue = name.toLowerCase();
	
		return this.storeDtoList.filter(option => option.productName.toLowerCase().includes(filterValue));
	  }

	

	getCustomersLookUp() {
		this.customerService.getCustomerList().subscribe(data => {
			this.customerList = data;
			

			this.filteredOptionsWithCustomer = this.customerControl.valueChanges.pipe(
				startWith(''),
				map(value => {
				  const name = typeof value === 'string' ? value : value?.name;
				  return name ? this._filterCustomer(name as string) : this.customerList.slice();
				}))
		});
	}

	displayFnCustomer(data: Customer): string {
		return data && data.customerName ? data.customerName : '';
	  }
	
	  private _filterCustomer(name: string): Customer[] {
		const filterValue = name.toLowerCase();
	
		return this.customerList.filter(option => option.customerName.toLowerCase().includes(filterValue));
	  }

	getOrderDtoList() {
		this.orderService.getOrderDtoList().subscribe(data => {
			this.orderDtoList = data;
			this.dataSource = new MatTableDataSource(data);
			this.configDataTable();
			console.log(data, "order data");
		});
	}

	save() {
		this.orderAddForm.controls.productId.setValue(this.storeDtoControl.value.productId);		
		this.orderAddForm.controls.customerId.setValue(this.customerControl.value.id);		
		if (this.orderAddForm.valid) {
			this.order = Object.assign({}, this.orderAddForm.value)

			if (this.order.id == 0)
				this.addOrder();
			else
				this.updateOrder();
		}

	}

	addOrder() {
		this.order.createdUserId = this.authService.getUserId();
		this.order.lastUpdatedUserId = this.authService.getUserId();
		this.orderService.addOrder(this.order).subscribe(data => {
			this.getOrderDtoList();
			this.order = new Order();
			jQuery('#order').modal('hide');
			this.alertifyService.success(data);
			this.clearFormGroup(this.orderAddForm);

		})

	}

	updateOrder() {
		this.order.lastUpdatedUserId = this.authService.getUserId();
		this.orderService.updateOrder(this.order).subscribe(data => {
			var index = this.orderDtoList.findIndex(x => x.id == this.order.id);
			this.orderDtoList[index] = this.order;
			this.getOrderDtoList();
			this.dataSource = new MatTableDataSource(this.orderDtoList);
			this.configDataTable();
			this.order = new Order();
			jQuery('#order').modal('hide');
			this.alertifyService.success(data);
			this.clearFormGroup(this.orderAddForm);

		})

	}

	createOrderAddForm() {
		this.orderAddForm = this.formBuilder.group({
			id: [0],
			createdUserId: [0],
			createdDate: [Date.now],
			lastUpdatedUserId: [0],
			lastUpdatedDate: [Date.now],
			status: [true],
			isDeleted: [false],
			customerId: [0, Validators.required],
			productId: [0, Validators.required],
			stock: [0, Validators.required]
		})
	}

	deleteOrder(orderId: number) {
		this.orderService.deleteOrder(orderId).subscribe(data => {
			this.alertifyService.success(data.toString());
			this.orderDtoList = this.orderDtoList.filter(x => x.id != orderId);
			this.dataSource = new MatTableDataSource(this.orderDtoList);
			this.configDataTable();
		})
	}

	getOrderById(orderId: number) {
		this.clearFormGroup(this.orderAddForm);
		this.orderService.getOrderById(orderId).subscribe(data => {
			this.order = data;
			this.orderAddForm.patchValue(data);
		})
	}


	clearFormGroup(group: FormGroup) {

		group.markAsUntouched();
		group.reset();

		Object.keys(group.controls).forEach(key => {
			group.get(key).setErrors(null);
			if (key == 'id')
				group.get(key).setValue(0);
		});
	}

	checkClaim(claim: string): boolean {
		return this.authService.claimGuard(claim)
	}

	configDataTable(): void {
		this.dataSource.paginator = this.paginator;
		this.dataSource.sort = this.sort;
	}

	applyFilter(event: Event) {
		const filterValue = (event.target as HTMLInputElement).value;
		this.dataSource.filter = filterValue.trim().toLowerCase();

		if (this.dataSource.paginator) {
			this.dataSource.paginator.firstPage();
		}
	}





}
