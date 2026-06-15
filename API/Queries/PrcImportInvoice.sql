select
    bu.Number as BillNumber,
    bu.Date as [Date],
    cu.CustomerName as Cust_Name,
    bu.Notes as Bill_Note,
    bi.Notes as Item_Notes,
    mt.Name as Item_Name,
    bi.Qty,
    bi.Price,
    (bi.Qty * bi.Price) as Total,

    sum(bi.Qty * bi.Price)
    over(partition by cu.CustomerName) as CustomerTotal

from bi000 bi
inner join bu000 bu on bu.GUID = bi.ParentGUID
inner join cu000 cu on cu.GUID = bu.CustGUID
inner join bt000 bt on bt.GUID = bu.TypeGUID
inner join mt000 mt on mt.GUID = bi.MatGUID
where cast(bu.Date as date)
      between cast(@StartDate as date)
      and cast(@EndDate as date)
      and bu.TypeGUID = @Type

order by cu.CustomerName, bu.Number