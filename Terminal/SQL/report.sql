SELECT
  SUM(CASE WHEN ss.Amount > 0 AND
    ss.Type = 1 THEN ss.Amount ELSE 0 END) AS A,
  SUM(CASE WHEN ss.Amount > 0 AND
    ss.Type = 1 THEN ss.ShopBaseCost ELSE 0 END) AS SBC,
  SUM(CASE WHEN ss.Amount > 0 AND
    ss.Type = 1 THEN ss.ShopCost ELSE 0 END) AS SC,
  SUM(CASE WHEN ss.Amount < 0 AND
    ss.Type = 1 THEN ss.Amount ELSE 0 END) AS AR,
  SUM(CASE WHEN ss.Amount < 0 AND
    ss.Type = 1 THEN ss.ShopBaseCost ELSE 0 END) AS SBCR,
  SUM(CASE WHEN ss.Amount < 0 AND
    ss.Type = 1 THEN ss.ShopCost ELSE 0 END) AS SCR,
  ifnull(ss.Resourcename, '') AS N,
  SUM(CASE WHEN ss.Type = 2 THEN ss.Amount ELSE 0 END) AS ARA,
  SUM(CASE WHEN ss.Type = 2 THEN ss.ShopBaseCost ELSE 0 END) AS SBCRA,
  ss.ICI AS ICI,
  ss.OK AS OK,
  ss.PaymentType AS PT
FROM (SELECT
    SUM(Amount) AS Amount,
    SUM(ShopBaseCost) AS ShopBaseCost,
    SUM(ShopCost) AS ShopCost,
    ResourceKey,
    ResourceName,
    1 AS Type,
    CASE WHEN DerivedType IN (%PaymentType%) THEN DerivedType ELSE 32766 END AS PaymentType,
    CASE WHEN IssuerCardId IN (%IssuerList%) THEN IssuerCardId ELSE %ElseIssuer% END AS ICI,
    CASE WHEN %DevideOrg%=1 THEN OrganisationKey ELSE -1 END AS OK
  FROM selling
  WHERE ShiftKey = %ShiftKey%
  GROUP BY Checknumber,
           ResourceKey,
           IssuerCardId,
           OrganisationKey

  UNION ALL

  SELECT
    SUM(Amount) AS Amount,
    SUM(ShopBaseCost) AS ShopBaseCost,
    SUM(ShopCost) AS ShopCost,
    ResourceKey,
    ResourceName,
    2 AS Type,
    CASE WHEN DerivedType IN (%PaymentType%) THEN DerivedType ELSE 32766 END AS PaymentType,
    CASE WHEN IssuerCardId IN (%IssuerList%) THEN IssuerCardId ELSE %ElseIssuer% END AS ICI,
    CASE WHEN %DevideOrg%=1 THEN OrganisationKey ELSE -1 END AS OK
  FROM selling_ignore
  WHERE ShiftKey = %ShiftKey%
  GROUP BY Checknumber,
           ResourceKey,
           IssuerCardId,
           OrganisationKey) ss
GROUP BY ss.PaymentType,
	 ss.ICI,
	 ss.OK,
         ss.ResourceKey
ORDER BY ss.PaymentType, ss.ICI,
ss.OK,
ss.ResourceKey