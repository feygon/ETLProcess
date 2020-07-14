select Docid, custname, AccountID, MemberID, DocCreateDate
from Documents doc
left join Submissions sub on sub.submid = doc.submid
left join Subtype ST on ST.subtypeid = sub.subtypeid
left join Customer CST on CST.custid = ST.custid
where custname = @custName
and Datediff(day, @startDate, doc.CreateDate) >= 0